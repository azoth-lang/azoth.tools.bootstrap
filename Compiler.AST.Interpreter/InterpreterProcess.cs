using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.Async;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout.BoundedLists;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter;

public class InterpreterProcess
{
    public static InterpreterProcess StartEntryPoint(Package package)
    {
        if (package.EntryPoint is null)
            throw new ArgumentException("Cannot execute package without an entry point");

        return new InterpreterProcess(package, runTests: false);
    }

    public static InterpreterProcess StartTests(Package package)
        => new(package, runTests: true);

    private readonly Package package;
    private readonly Task executionTask;
    private readonly FixedDictionary<FunctionSymbol, IConcreteFunctionInvocableDeclaration> functions;
    private readonly FixedDictionary<ConstructorSymbol, IConstructorDeclaration?> constructors;
    private readonly FixedDictionary<ObjectTypeSymbol, IClassDeclaration> classes;
    private readonly IClassDeclaration stringClass;
    private readonly IConstructorDeclaration stringConstructor;
    private readonly IClassDeclaration? rangeClass;
    private readonly ConstructorSymbol? rangeConstructor;
    private byte? exitCode;
    private readonly MemoryStream standardOutput = new();
    private readonly TextWriter standardOutputWriter;
    private readonly MethodSignatureCache methodSignatures = new();
    private readonly ConcurrentDictionary<IClassDeclaration, VTable> vTables = new();

    private InterpreterProcess(Package package, bool runTests)
    {
        this.package = package;
        var allDeclarations = GetAllDeclarations(package, runTests ? r => r.Declarations.Concat(r.TestingDeclarations) : r => r.Declarations);
        functions = allDeclarations
                    .OfType<IConcreteFunctionInvocableDeclaration>()
                    .ToFixedDictionary(f => f.Symbol);

        classes = allDeclarations.OfType<IClassDeclaration>()
                                 .ToFixedDictionary(c => c.Symbol);
        stringClass = classes.Values.Single(c => c.Symbol.Name == "String");
        stringConstructor = stringClass.Members.OfType<IConstructorDeclaration>().Single(c => c.Parameters.Count == 3);
        rangeClass = classes.Values.SingleOrDefault(c => c.Symbol.Name == "range");
        rangeConstructor = rangeClass?.Members.OfType<IConstructorDeclaration>().SingleOrDefault(c => c.Parameters.Count == 2)?.Symbol;
        var defaultConstructorSymbols = allDeclarations
                                        .OfType<IClassDeclaration>()
                                        .Select(c => c.DefaultConstructorSymbol).WhereNotNull();
        constructors = defaultConstructorSymbols
                       .Select(c => (c, default(IConstructorDeclaration)))
                       .Concat(allDeclarations
                               .OfType<IConstructorDeclaration>()
                               .Select(c => (c.Symbol, (IConstructorDeclaration?)c)))
                       .ToFixedDictionary();

        // TODO pointing both of these to a memory stream is probably wrong. Need something that acts like a pipe.
        standardOutputWriter = new StreamWriter(standardOutput, Encoding.UTF8, leaveOpen: true);
        StandardOutput = new StreamReader(standardOutput, Encoding.UTF8);


        executionTask = runTests ? Task.Run(RunTestsAsync) : Task.Run(CallEntryPointAsync);
    }

    private static List<IDeclaration> GetAllDeclarations(Package package, Func<Package, IEnumerable<IDeclaration>> getDeclarations)
        => getDeclarations(package).Concat(package.References.SelectMany(getDeclarations)).ToList();

    private async Task CallEntryPointAsync()
    {
        try
        {
            var entryPoint = package.EntryPoint!;
            var arguments = new List<AzothValue>();
            foreach (var parameterType in entryPoint.Symbol.ParameterTypes)
                arguments.Add(await ConstructMainParameterAsync(parameterType.Type));

            var returnValue = await CallFunctionAsync(entryPoint, arguments).ConfigureAwait(false);
            // Flush any buffered output
            await standardOutputWriter.FlushAsync().ConfigureAwait(false);
            var returnType = entryPoint.Symbol.ReturnType;
            if (returnType == ReturnType.Void)
                exitCode = 0;
            else if (returnType.Type == DataType.Byte)
                exitCode = returnValue.ByteValue;
            else
                throw new InvalidOperationException($"Main function cannot have return type {returnType.ToILString()}");
        }
        finally
        {
            await standardOutputWriter.DisposeAsync().ConfigureAwait(false);
            standardOutput.Position = 0;
        }
    }

    private async Task RunTestsAsync()
    {
        try
        {
            var testFunctions = package.TestingDeclarations.OfType<IFunctionDeclaration>()
                                       .Where(f => f.Attributes.Any(IsTestAttribute)).ToFixedSet();

            await standardOutputWriter.WriteLineAsync($"Testing {package.Symbol.Name} package...");
            await standardOutputWriter.WriteLineAsync($"  Found {testFunctions.Count} tests");
            await standardOutputWriter.WriteLineAsync();

            int failed = 0;

            foreach (var function in testFunctions)
            {
                // TODO check that return type is void
                var symbol = function.Symbol;
                await standardOutputWriter.WriteLineAsync($"{symbol.ContainingSymbol.ToILString()}.{symbol.Name} ...");
                try
                {
                    await CallFunctionAsync(function, Enumerable.Empty<AzothValue>()).ConfigureAwait(false);
                    await standardOutputWriter.WriteLineAsync("  passed");
                }
                catch (Abort ex)
                {
                    await standardOutputWriter.WriteLineAsync("  FAILED: " + ex.Message);
                    failed += 1;
                }
            }

            await standardOutputWriter.WriteLineAsync();
            await standardOutputWriter.WriteLineAsync($"Tested {package.Symbol.Name} package");
            await standardOutputWriter.WriteLineAsync($"{failed} failed tests out of {testFunctions.Count} total");

            // Flush any buffered output
            await standardOutputWriter.FlushAsync().ConfigureAwait(false);
        }
        finally
        {
            await standardOutputWriter.DisposeAsync().ConfigureAwait(false);
            standardOutput.Position = 0;
        }
    }

    private static bool IsTestAttribute(IAttribute attribute1)
        => attribute1.ReferencedSymbol.Name.Text == "Test_Attribute";

    private async Task<AzothValue> ConstructMainParameterAsync(DataType parameterType)
    {
        if (parameterType is not ObjectType { TypeArguments.Count: 0 } type)
            throw new InvalidOperationException(
                $"Parameter to main of type {parameterType.ToILString()} not supported");

        // TODO further restrict what can be passed to main

        var @class = classes.Values.Single(c => c.Symbol.DeclaresType == type.DeclaredType);
        var constructorSymbol = @class.DefaultConstructorSymbol
            ?? @class.Members.OfType<IConstructorDeclaration>().Select(c => c.Symbol)
                     .Single(c => c.Arity == 0);
        return await ConstructClass(@class, constructorSymbol, Enumerable.Empty<AzothValue>());
    }

    internal async Task<AzothValue> CallFunctionAsync(IConcreteFunctionInvocableDeclaration function, IEnumerable<AzothValue> arguments)
    {
        try
        {
            var variables = new LocalVariableScope();
            foreach (var (arg, symbol) in arguments.Zip(function.Parameters.Select(p => p.Symbol)))
                variables.Add(symbol, arg);

            foreach (var statement in function.Body.Statements)
                await ExecuteAsync(statement, variables).ConfigureAwait(false);
            return default;
        }
        catch (Return @return)
        {
            return @return.Value;
        }
    }

    private async Task<AzothValue> ConstructClass(
        IClassDeclaration @class,
        ConstructorSymbol constructorSymbol,
        IEnumerable<AzothValue> arguments)
    {
        var vTable = vTables.GetOrAdd(@class, CreateVTable);
        var self = AzothValue.Object(new AzothObject(vTable));
        return await CallConstructorAsync(@class, constructorSymbol, self, arguments);
    }

    private async Task<AzothValue> CallConstructorAsync(
        IClassDeclaration @class,
        ConstructorSymbol constructorSymbol,
        AzothValue self,
        IEnumerable<AzothValue> arguments)
    {
        // TODO run field initializers
        var constructor = constructors[constructorSymbol];
        // Default constructor is null
        if (constructor is null) return await CallDefaultConstructorAsync(@class, self);
        return await CallConstructorAsync(constructor, self, arguments).ConfigureAwait(false);
    }

    private async ValueTask<AzothValue> CallConstructorAsync(
        IConstructorDeclaration constructor,
        AzothValue self,
        IEnumerable<AzothValue> arguments)
    {
        try
        {
            var variables = new LocalVariableScope();
            variables.Add(constructor.SelfParameter.Symbol, self);
            foreach (var (arg, parameter) in arguments.Zip(constructor.Parameters))
                switch (parameter)
                {
                    default:
                        throw ExhaustiveMatch.Failed(parameter);
                    case IFieldParameter fieldParameter:
                        self.ObjectValue[fieldParameter.ReferencedSymbol.Name] = arg;
                        break;
                    case INamedParameter p:
                        variables.Add(p.Symbol, arg);
                        break;
                }

            foreach (var statement in constructor.Body.Statements)
                await ExecuteAsync(statement, variables).ConfigureAwait(false);
            return self;
        }
        catch (Return)
        {
            return self;
        }
    }

    /// <summary>
    /// Call the implicit default constructor for a type that has no constructors.
    /// </summary>
    private async ValueTask<AzothValue> CallDefaultConstructorAsync(IClassDeclaration @class, AzothValue self)
    {
        // Initialize fields to default values
        var fields = @class.Members.OfType<IFieldDeclaration>();
        foreach (var field in fields)
            self.ObjectValue[field.Symbol.Name] = new AzothValue();

        if (@class.BaseClass is IClassDeclaration baseClass)
        {
            var noArgConstructorSymbol = NoArgConstructorSymbol(baseClass);
            await CallConstructorAsync(baseClass, noArgConstructorSymbol, self, Enumerable.Empty<AzothValue>());
        }

        return self;
    }

    private static ConstructorSymbol NoArgConstructorSymbol(IClassDeclaration baseClass)
    {
        return baseClass.DefaultConstructorSymbol
               ?? baseClass.Members.OfType<IConstructorDeclaration>().Select(c => c.Symbol)
                           .Single(c => c.Arity == 0);
    }

    private async ValueTask<AzothValue> CallMethodAsync(
        IMethodDeclaration method,
        AzothValue self,
        IEnumerable<AzothValue> arguments)
    {
        if (method is not IConcreteMethodDeclaration concreteMethod)
            throw new InvalidOperationException($"Can't call abstract method {method}");

        try
        {
            var variables = new LocalVariableScope();
            variables.Add(method.SelfParameter.Symbol, self);
            foreach (var (arg, symbol) in arguments.Zip(method.Parameters.Select(p => p.Symbol)))
                variables.Add(symbol, arg);

            foreach (var statement in concreteMethod.Body.Statements)
                await ExecuteAsync(statement, variables).ConfigureAwait(false);
            return default;
        }
        catch (Return @return)
        {
            return @return.Value;
        }
    }

    private async ValueTask<AzothValue> ExecuteAsync(IStatement statement, LocalVariableScope variables)
    {
        switch (statement)
        {
            default:
                throw ExhaustiveMatch.Failed(statement);
            case IExpressionStatement s:
                await ExecuteAsync(s.Expression, variables).ConfigureAwait(false);
                break;
            case IVariableDeclarationStatement d:
            {
                var initialValue = d.Initializer is null
                    ? AzothValue.None
                    : await ExecuteAsync(d.Initializer, variables).ConfigureAwait(false);
                variables.Add(d.Symbol, initialValue);
                break;
            }
            case IResultStatement r:
                return await ExecuteAsync(r.Expression, variables).ConfigureAwait(false);
        }
        return AzothValue.None;
    }

    private async ValueTask<AzothValue> ExecuteAsync(IExpression expression, LocalVariableScope variables)
    {
        switch (expression)
        {
            default:
                throw ExhaustiveMatch.Failed(expression);
            case IIdExpression exp:
                return await ExecuteAsync(exp.Referent, variables);
            case IMoveExpression exp:
                return await ExecuteAsync(exp.Referent, variables);
            case IFreezeExpression exp:
                return await ExecuteAsync(exp.Referent, variables);
            case INoneLiteralExpression _:
                return AzothValue.None;
            case IReturnExpression exp:
                if (exp.Value is null) throw new Return();
                throw new Return(await ExecuteAsync(exp.Value, variables).ConfigureAwait(false));
            case IImplicitNumericConversionExpression exp:
            {
                var value = await ExecuteAsync(exp.Expression, variables).ConfigureAwait(false);
                return value.Convert(exp.Expression.DataType, exp.ConvertToType, false);
            }
            case IExplicitNumericConversionExpression exp:
            {
                var value = await ExecuteAsync(exp.Expression, variables).ConfigureAwait(false);
                return value.Convert(exp.Expression.DataType, exp.ConvertToType, exp.IsOptional);
            }
            case IIntegerLiteralExpression exp:
                return AzothValue.Int(exp.Value);
            case IFunctionInvocationExpression exp:
            {
                var arguments = await ExecuteArgumentsAsync(exp.Arguments, variables).ConfigureAwait(false);
                var functionSymbol = exp.ReferencedSymbol;
                if (functionSymbol.Package == Intrinsic.SymbolTree.Package)
                    return await CallIntrinsicAsync(functionSymbol, arguments).ConfigureAwait(false);
                return await CallFunctionAsync(functions[functionSymbol], arguments).ConfigureAwait(false);
            }
            case IFunctionReferenceInvocationExpression exp:
            {
                var function = variables[exp.ReferencedSymbol];
                var arguments = await ExecuteArgumentsAsync(exp.Arguments, variables).ConfigureAwait(false);
                return await function.FunctionReferenceValue.CallAsync(arguments).ConfigureAwait(false);
            }
            case IBoolLiteralExpression exp:
                return AzothValue.Bool(exp.Value);
            case IIfExpression exp:
            {
                var condition = await ExecuteAsync(exp.Condition, variables).ConfigureAwait(false);
                if (condition.BoolValue)
                    return await ExecuteBlockOrResultAsync(exp.ThenBlock, variables).ConfigureAwait(false);
                if (exp.ElseClause is not null)
                    return await ExecuteElseAsync(exp.ElseClause, variables).ConfigureAwait(false);
                return AzothValue.None;
            }
            case IVariableNameExpression exp:
                return variables[exp.ReferencedSymbol];
            case IFunctionNameExpression exp:
                return AzothValue.FunctionReference(new ConcreteFunctionReference(this, functions[exp.ReferencedSymbol]));
            case IBlockExpression block:
            {
                var blockVariables = new LocalVariableScope(variables);
                AzothValue lastValue = AzothValue.None;
                foreach (var statement in block.Statements)
                {
                    if (statement is IResultStatement resultStatement)
                        return await ExecuteAsync(resultStatement.Expression, blockVariables).ConfigureAwait(false);
                    lastValue = await ExecuteAsync(statement, blockVariables).ConfigureAwait(false);
                }
                return lastValue;
            }
            case ILoopExpression exp:
                try
                {
                    for (; ; )
                    {
                        try
                        {
                            await ExecuteAsync(exp.Block, variables).ConfigureAwait(false);
                        }
                        catch (Next)
                        {
                            continue;
                        }
                    }
                }
                catch (Break @break)
                {
                    return @break.Value;
                }
            case IWhileExpression exp:
                try
                {
                    for (; ; )
                    {
                        var condition = await ExecuteAsync(exp.Condition, variables).ConfigureAwait(false);
                        if (!condition.BoolValue)
                            return AzothValue.None;
                        try
                        {
                            await ExecuteAsync(exp.Block, variables).ConfigureAwait(false);
                        }
                        catch (Next)
                        {
                            continue;
                        }
                    }
                }
                catch (Break @break)
                {
                    return @break.Value;
                }
            case IBreakExpression exp:
                if (exp.Value is null) throw new Break();
                throw new Break(await ExecuteAsync(exp.Value, variables).ConfigureAwait(false));
            case INextExpression _:
                throw new Next();
            case IAssignmentExpression exp:
            {
                // TODO this evaluates the left hand side twice for compound operators
                var value = exp.Operator switch
                {
                    AssignmentOperator.Simple =>
                        // TODO the expression being assigned into is supposed to be evaluated first
                        await ExecuteAsync(exp.RightOperand, variables).ConfigureAwait(false),
                    AssignmentOperator.Plus
                        => await AddAsync(exp.LeftOperand, exp.RightOperand, variables).ConfigureAwait(false),
                    AssignmentOperator.Minus
                        => await SubtractAsync(exp.LeftOperand, exp.RightOperand, variables).ConfigureAwait(false),
                    AssignmentOperator.Asterisk
                        => await MultiplyAsync(exp.LeftOperand, exp.RightOperand, variables).ConfigureAwait(false),
                    AssignmentOperator.Slash
                        => await DivideAsync(exp.LeftOperand, exp.RightOperand, variables).ConfigureAwait(false),
                    _ => throw ExhaustiveMatch.Failed(exp.Operator)
                };
                await ExecuteAssignmentAsync(exp.LeftOperand, value, variables).ConfigureAwait(false);
                return value;
            }
            case IBinaryOperatorExpression exp:
                switch (exp.Operator)
                {
                    default:
                        throw ExhaustiveMatch.Failed();
                    case BinaryOperator.Plus:
                        return await AddAsync(exp.LeftOperand, exp.RightOperand, variables).ConfigureAwait(false);
                    case BinaryOperator.Minus:
                        return await SubtractAsync(exp.LeftOperand, exp.RightOperand, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.Asterisk:
                        return await MultiplyAsync(exp.LeftOperand, exp.RightOperand, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.Slash:
                        return await DivideAsync(exp.LeftOperand, exp.RightOperand, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.EqualsEquals:
                        return AzothValue.Bool(await EqualsAsync(exp.LeftOperand, exp.RightOperand, variables)
                            .ConfigureAwait(false));
                    case BinaryOperator.NotEqual:
                        return AzothValue.Bool(!await EqualsAsync(exp.LeftOperand, exp.RightOperand, variables)
                            .ConfigureAwait(false));
                    case BinaryOperator.LessThan:
                        return AzothValue.Bool(await CompareAsync(exp.LeftOperand, exp.RightOperand, variables)
                            .ConfigureAwait(false) < 0);
                    case BinaryOperator.LessThanOrEqual:
                        return AzothValue.Bool(await CompareAsync(exp.LeftOperand, exp.RightOperand, variables)
                            .ConfigureAwait(false) <= 0);
                    case BinaryOperator.GreaterThan:
                        return AzothValue.Bool(await CompareAsync(exp.LeftOperand, exp.RightOperand, variables)
                            .ConfigureAwait(false) > 0);
                    case BinaryOperator.GreaterThanOrEqual:
                        return AzothValue.Bool(await CompareAsync(exp.LeftOperand, exp.RightOperand, variables)
                            .ConfigureAwait(false) >= 0);
                    case BinaryOperator.And:
                    {
                        var left = await ExecuteAsync(exp.LeftOperand, variables).ConfigureAwait(false);
                        if (!left.BoolValue) return AzothValue.Bool(false);
                        return await ExecuteAsync(exp.RightOperand, variables).ConfigureAwait(false);
                    }
                    case BinaryOperator.Or:
                    {
                        var left = await ExecuteAsync(exp.LeftOperand, variables).ConfigureAwait(false);
                        if (left.BoolValue) return AzothValue.Bool(true);
                        return await ExecuteAsync(exp.RightOperand, variables).ConfigureAwait(false);
                    }
                    case BinaryOperator.DotDot:
                    case BinaryOperator.LessThanDotDot:
                    case BinaryOperator.DotDotLessThan:
                    case BinaryOperator.LessThanDotDotLessThan:
                    {
                        var left = await ExecuteAsync(exp.LeftOperand, variables).ConfigureAwait(false);
                        if (!exp.Operator.RangeInclusiveOfStart()) left = left.Increment(DataType.Int);
                        var right = await ExecuteAsync(exp.RightOperand, variables).ConfigureAwait(false);
                        if (exp.Operator.RangeInclusiveOfEnd()) right = right.Increment(DataType.Int);
                        return await ConstructClass(rangeClass!, rangeConstructor!, new[] { left, right });
                    }
                    case BinaryOperator.QuestionQuestion:
                        throw new NotImplementedException($"Operator `{exp.Operator}`");
                }
            case IUnaryOperatorExpression exp:
                return exp.Operator switch
                {
                    UnaryOperator.Not => AzothValue.Bool(
                        !(await ExecuteAsync(exp.Operand, variables).ConfigureAwait(false)).BoolValue),
                    UnaryOperator.Minus => await NegateAsync(exp.Operand, variables).ConfigureAwait(false),
                    UnaryOperator.Plus => await ExecuteAsync(exp.Operand, variables).ConfigureAwait(false),
                    _ => throw ExhaustiveMatch.Failed(exp.Operator)
                };
            case IMethodInvocationExpression exp:
            {
                var self = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                var arguments = await ExecuteArgumentsAsync(exp.Arguments, variables).ConfigureAwait(false);
                var methodSymbol = exp.ReferencedSymbol;
                if (methodSymbol.Package == Intrinsic.SymbolTree.Package)
                    return await CallIntrinsicAsync(methodSymbol, self, arguments);
                var methodSignature = methodSignatures[methodSymbol];

                var selfType = exp.Context.DataType;
                switch (selfType)
                {
                    case EmptyType _:
                    case UnknownType _:
                    case BoolType _:
                    case OptionalType _:
                    case GenericParameterType _:
                    case FunctionType _:
                        throw new InvalidOperationException($"Can't call {methodSignature} on {selfType}");
                    case ReferenceType _:
                        var vtable = self.ObjectValue.VTable;
                        var method = vtable[methodSignature];
                        return await CallMethodAsync(method, self, arguments).ConfigureAwait(false);
                    case NumericType numericType:
                        return methodSignature.Name.Text switch
                        {
                            "remainder" => Remainder(self, arguments[0], numericType),
                            "to_display_string" => await ToDisplayStringAsync(self, numericType),
                            _ => throw new InvalidOperationException($"Can't call {methodSignature} on {selfType}")
                        };
                    default:
                        throw ExhaustiveMatch.Failed(selfType);
                }
            }
            case INewObjectExpression exp:
            {
                var arguments = await ExecuteArgumentsAsync(exp.Arguments, variables).ConfigureAwait(false);
                var constructorSymbol = exp.ReferencedSymbol;
                var objectTypeSymbol = constructorSymbol.ContainingSymbol;
                if (objectTypeSymbol.Package == Intrinsic.SymbolTree.Package)
                    return await CallIntrinsicAsync(constructorSymbol, arguments).ConfigureAwait(false);
                var @class = classes[objectTypeSymbol];
                return await ConstructClass(@class, constructorSymbol, arguments);
            }
            case IShareExpression exp:
                // TODO do share expressions make sense in Azoth?
                return await ExecuteAsync(exp.Referent, variables).ConfigureAwait(false);
            case ISelfExpression exp:
                return variables[exp.ReferencedSymbol];
            case IImplicitOptionalConversionExpression exp:
                return await ExecuteAsync(exp.Expression, variables).ConfigureAwait(false);
            case IStringLiteralExpression exp:
            {
                // Call the constructor of the string class
                var value = exp.Value;
                return await ConstructStringAsync(value);
            }
            case IUnsafeExpression exp:
                return await ExecuteAsync(exp.Expression, variables).ConfigureAwait(false);
            case IFieldAccessExpression exp:
            {
                var obj = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                return obj.ObjectValue[exp.ReferencedSymbol.Name];
            }
            case IForeachExpression exp:
            {
                var iterable = await ExecuteAsync(exp.InExpression, variables).ConfigureAwait(false);
                var loopVariable = exp.Symbol;
                // Call `iterable.iterate()` if it exists
                AzothValue iterator;
                if (exp.IterateMethod is not null)
                {
                    var iterateMethodSignature = methodSignatures[exp.IterateMethod];
                    var iterableVTable = iterable.ObjectValue.VTable;
                    var iterateMethod = iterableVTable[iterateMethodSignature];
                    iterator = await CallMethodAsync(iterateMethod, iterable, Enumerable.Empty<AzothValue>()).ConfigureAwait(false);
                }
                else
                    iterator = iterable;

                var nextMethodSignature = methodSignatures[exp.NextMethod];
                var iteratorVTable = iterator.ObjectValue.VTable;
                var nextMethod = iteratorVTable[nextMethodSignature];

                try
                {
                    while (true)
                    {
                        var value = await CallMethodAsync(nextMethod, iterator, Enumerable.Empty<AzothValue>()).ConfigureAwait(false);
                        if (value.IsNone) break;
                        try
                        {
                            var loopVariables = new LocalVariableScope(variables);
                            loopVariables.Add(loopVariable, value);
                            await ExecuteAsync(exp.Block, loopVariables).ConfigureAwait(false);
                        }
                        catch (Next)
                        {
                            // continue
                        }
                    }
                    return AzothValue.None;
                }
                catch (Break @break)
                {
                    return @break.Value;
                }
            }
            case IRecoverExpression exp:
                return await ExecuteAsync(exp.Value, variables).ConfigureAwait(false);
            case IImplicitLiftedConversionExpression exp:
            {
                var value = await ExecuteAsync(exp.Expression, variables).ConfigureAwait(false);
                if (value.IsNone) return value;
                // TODO handle other lifted conversions
                return value.Convert(exp.Expression.DataType, (NumericType)exp.ConvertToType.Referent, false);
            }
            case IPatternMatchExpression exp:
            {
                var value = await ExecuteAsync(exp.Referent, variables).ConfigureAwait(false);
                return await ExecuteMatchAsync(value, exp.Pattern, variables);
            }
            case IAsyncBlockExpression exp:
            {
                var asyncScope = new AsyncScope();
                var blockVariables = new LocalVariableScope(variables, asyncScope);
                try
                {
                    return await ExecuteAsync(exp.Block, blockVariables);
                }
                finally
                {
                    await asyncScope.ExitAsync();
                }
            }
            case IAsyncStartExpression exp:
            {
                if (variables.AsyncScope is not AsyncScope asyncScope)
                    throw new InvalidOperationException("Cannot execute `go` or `do` expression outside of an async scope.");

                var task = exp.Scheduled
                    ? Task.Run(async () => await ExecuteAsync(exp.Expression, variables))
                    : ExecuteAsync(exp.Expression, variables).AsTask();

                asyncScope.Add(task);

                return AzothValue.Promise(task);
            }
            case IAwaitExpression exp:
            {
                var value = await ExecuteAsync(exp.Expression, variables).ConfigureAwait(false);

                return await value.PromiseValue.ConfigureAwait(false);
            }
        }
    }

    private async ValueTask<AzothValue> ExecuteMatchAsync(
        AzothValue value,
        IPattern pattern,
        LocalVariableScope variables)
    {
        switch (pattern)
        {
            default:
                throw ExhaustiveMatch.Failed(pattern);
            case IBindingContextPattern pat:
                throw new NotImplementedException();
            case IBindingPattern pat:
                variables.Add(pat.Symbol, value);
                return AzothValue.Bool(true);
            case IOptionalPattern pat:
                if (value.IsNone)
                    return AzothValue.Bool(false);
                return await ExecuteMatchAsync(value, pat.Pattern, variables);
        }
    }

    private async ValueTask<AzothValue> ConstructStringAsync(string value)
    {
        var bytes = new RawBoundedByteList(Encoding.UTF8.GetBytes(value));
        var arguments = new List<AzothValue>
        {
            // bytes: const Raw_Bounded_List[byte]
            AzothValue.RawBoundedList(bytes),
            // start: size
            AzothValue.Size(0),
            // byte_count: size
            AzothValue.Size(bytes.Count),
        };
        var vTable = vTables.GetOrAdd(stringClass, CreateVTable);
        var self = AzothValue.Object(new AzothObject(vTable));
        return await CallConstructorAsync(stringConstructor, self, arguments).ConfigureAwait(false);
    }

    private async ValueTask<AzothValue> CallIntrinsicAsync(FunctionSymbol function, List<AzothValue> arguments)
    {
        if (function == Intrinsic.PrintRawUtf8Bytes)
        {
            string str = RawUtf8BytesToString(arguments);
            await standardOutputWriter.WriteAsync(str).ConfigureAwait(false);
            return AzothValue.None;
        }
        if (function == Intrinsic.AbortRawUtf8Bytes)
        {
            string message = RawUtf8BytesToString(arguments);
            throw new Abort(message);
        }
        if (function == Intrinsic.IdentityHash)
        {
            var value = arguments[0].ObjectValue;
            return AzothValue.U64((ulong)value.GetHashCode());
        }
        throw new NotImplementedException($"Intrinsic {function}");
    }

    private static string RawUtf8BytesToString(List<AzothValue> arguments)
    {
        var bytes = (RawBoundedByteList)arguments[0].RawBoundedListValue;
        var start = arguments[1].SizeValue;
        var byteCount = arguments[2].SizeValue;
        var message = bytes.Utf8GetString(start, byteCount);
        return message;
    }

    private static ValueTask<AzothValue> CallIntrinsicAsync(ConstructorSymbol constructor, List<AzothValue> arguments)
    {
        if (constructor == Intrinsic.NewRawBoundedList)
        {
            var listType = constructor.ContainingSymbol.DeclaresType.GenericParameterTypes[0];
            nuint capacity = arguments[0].SizeValue;
            IRawBoundedList list;
            if (listType == DataType.Byte)
                list = new RawBoundedByteList(capacity);
            else
                list = new RawBoundedList(capacity);
            return ValueTask.FromResult(AzothValue.RawBoundedList(list));
        }

        throw new NotImplementedException($"Intrinsic {constructor}");
    }

    private static ValueTask<AzothValue> CallIntrinsicAsync(
        MethodSymbol method,
        AzothValue self,
        List<AzothValue> arguments)
    {
        if (method == Intrinsic.GetRawBoundedListCapacity)
            return ValueTask.FromResult(AzothValue.Size(self.RawBoundedListValue.Capacity));
        if (method == Intrinsic.GetRawBoundedListCount)
            return ValueTask.FromResult(AzothValue.Size(self.RawBoundedListValue.Count));
        if (method == Intrinsic.RawBoundedListAdd)
        {
            self.RawBoundedListValue.Add(arguments[0]);
            return ValueTask.FromResult(AzothValue.None);
        }
        if (method == Intrinsic.RawBoundedListAt)
            return ValueTask.FromResult(self.RawBoundedListValue.At(arguments[0].SizeValue));
        if (method == Intrinsic.RawBoundedListSetAt)
        {
            self.RawBoundedListValue.Set(arguments[0].SizeValue, arguments[1]);
            return ValueTask.FromResult(AzothValue.None);
        }
        if (method == Intrinsic.RawBoundedListShrink)
        {
            self.RawBoundedListValue.Shrink(arguments[0].SizeValue);
            return ValueTask.FromResult(AzothValue.None);
        }

        throw new NotImplementedException($"Intrinsic {method}");
    }

    private VTable CreateVTable(IClassDeclaration @class)
        => new(@class, methodSignatures);

    private async ValueTask<List<AzothValue>> ExecuteArgumentsAsync(FixedList<IExpression> arguments, LocalVariableScope variables)
    {
        var values = new List<AzothValue>(arguments.Count);
        // Execute arguments in order
        foreach (var argument in arguments)
            values.Add(await ExecuteAsync(argument, variables).ConfigureAwait(false));
        return values;
    }

    private async ValueTask<AzothValue> AddAsync(IExpression leftExp, IExpression rightExp, LocalVariableScope variables)
    {
        if (leftExp.DataType != rightExp.DataType)
            throw new InvalidOperationException(
                $"Can't add expressions of type {leftExp.DataType.ToILString()} and {rightExp.DataType.ToILString()}");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = leftExp.DataType;
        if (type == DataType.Int) return AzothValue.Int(left.IntValue + right.IntValue);
        if (type == DataType.UInt) return AzothValue.Int(left.IntValue + right.IntValue);
        if (type == DataType.Int8) return AzothValue.I8((sbyte)(left.I8Value + right.I8Value));
        if (type == DataType.Byte) return AzothValue.Byte((byte)(left.ByteValue + right.ByteValue));
        if (type == DataType.Int16) return AzothValue.I16((short)(left.I16Value + right.I16Value));
        if (type == DataType.UInt16) return AzothValue.U16((ushort)(left.U16Value + right.U16Value));
        if (type == DataType.Int32) return AzothValue.I32(left.I32Value + right.I32Value);
        if (type == DataType.UInt32) return AzothValue.U32(left.U32Value + right.U32Value);
        if (type == DataType.Int64) return AzothValue.I64(left.I64Value + right.I64Value);
        if (type == DataType.UInt64) return AzothValue.U64(left.U64Value + right.U64Value);
        if (type == DataType.Offset) return AzothValue.Offset(left.OffsetValue + right.OffsetValue);
        if (type == DataType.Size) return AzothValue.Size(left.SizeValue + right.SizeValue);
        throw new NotImplementedException($"Add {type.ToILString()}");
    }

    private async ValueTask<AzothValue> SubtractAsync(IExpression leftExp, IExpression rightExp, LocalVariableScope variables)
    {
        // TODO check for negative values when subtracting unsigned
        if (leftExp.DataType != rightExp.DataType)
            throw new InvalidOperationException(
                $"Can't subtract expressions of type {leftExp.DataType} and {rightExp.DataType}");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = leftExp.DataType;
        if (type == DataType.Int) return AzothValue.Int(left.IntValue - right.IntValue);
        if (type == DataType.UInt) return AzothValue.Int(left.IntValue - right.IntValue);
        if (type == DataType.Int8) return AzothValue.I8((sbyte)(left.I8Value - right.I8Value));
        if (type == DataType.Byte) return AzothValue.Byte((byte)(left.ByteValue - right.ByteValue));
        if (type == DataType.Int16) return AzothValue.I16((short)(left.I16Value - right.I16Value));
        if (type == DataType.UInt16) return AzothValue.U16((ushort)(left.U16Value - right.U16Value));
        if (type == DataType.Int32) return AzothValue.I32(left.I32Value - right.I32Value);
        if (type == DataType.UInt32) return AzothValue.U32(left.U32Value - right.U32Value);
        if (type == DataType.Int64) return AzothValue.I64(left.I64Value - right.I64Value);
        if (type == DataType.UInt64) return AzothValue.U64(left.U64Value - right.U64Value);
        if (type == DataType.Offset) return AzothValue.Offset(left.OffsetValue - right.OffsetValue);
        if (type == DataType.Size) return AzothValue.Size(left.SizeValue - right.SizeValue);
        throw new NotImplementedException($"Subtract {type.ToILString()}");
    }

    private async ValueTask<AzothValue> MultiplyAsync(IExpression leftExp, IExpression rightExp, LocalVariableScope variables)
    {
        if (leftExp.DataType != rightExp.DataType)
            throw new InvalidOperationException(
                $"Can't multiply expressions of type {leftExp.DataType.ToILString()} and {rightExp.DataType.ToILString()}");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = leftExp.DataType;
        if (type == DataType.Int) return AzothValue.Int(left.IntValue * right.IntValue);
        if (type == DataType.UInt) return AzothValue.Int(left.IntValue * right.IntValue);
        if (type == DataType.Int8) return AzothValue.I8((sbyte)(left.I8Value * right.I8Value));
        if (type == DataType.Byte) return AzothValue.Byte((byte)(left.ByteValue * right.ByteValue));
        if (type == DataType.Int16) return AzothValue.I16((short)(left.I16Value * right.I16Value));
        if (type == DataType.UInt16) return AzothValue.U16((ushort)(left.U16Value * right.U16Value));
        if (type == DataType.Int32) return AzothValue.I32(left.I32Value * right.I32Value);
        if (type == DataType.UInt32) return AzothValue.U32(left.U32Value * right.U32Value);
        if (type == DataType.Int64) return AzothValue.I64(left.I64Value * right.I64Value);
        if (type == DataType.UInt64) return AzothValue.U64(left.U64Value * right.U64Value);
        if (type == DataType.Offset) return AzothValue.Offset(left.OffsetValue * right.OffsetValue);
        if (type == DataType.Size) return AzothValue.Size(left.SizeValue * right.SizeValue);
        throw new NotImplementedException($"Multiply {type.ToILString()}");
    }

    private async ValueTask<AzothValue> DivideAsync(IExpression leftExp, IExpression rightExp, LocalVariableScope variables)
    {
        if (leftExp.DataType != rightExp.DataType)
            throw new InvalidOperationException(
                $"Can't divide expressions of type {leftExp.DataType} and {rightExp.DataType}");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = leftExp.DataType;
        if (type == DataType.Int) return AzothValue.Int(left.IntValue / right.IntValue);
        if (type == DataType.UInt) return AzothValue.Int(left.IntValue / right.IntValue);
        if (type == DataType.Int8) return AzothValue.I8((sbyte)(left.I8Value / right.I8Value));
        if (type == DataType.Byte) return AzothValue.Byte((byte)(left.ByteValue / right.ByteValue));
        if (type == DataType.Int16) return AzothValue.I16((short)(left.I16Value / right.I16Value));
        if (type == DataType.UInt16) return AzothValue.U16((ushort)(left.U16Value / right.U16Value));
        if (type == DataType.Int32) return AzothValue.I32(left.I32Value / right.I32Value);
        if (type == DataType.UInt32) return AzothValue.U32(left.U32Value / right.U32Value);
        if (type == DataType.Int64) return AzothValue.I64(left.I64Value / right.I64Value);
        if (type == DataType.UInt64) return AzothValue.U64(left.U64Value / right.U64Value);
        if (type == DataType.Offset) return AzothValue.Offset(left.OffsetValue / right.OffsetValue);
        if (type == DataType.Size) return AzothValue.Size(left.SizeValue / right.SizeValue);
        throw new NotImplementedException($"Divide {type.ToILString()}");
    }

    private async ValueTask<bool> EqualsAsync(IExpression leftExp, IExpression rightExp, LocalVariableScope variables)
    {
        if (leftExp.DataType != rightExp.DataType)
            throw new InvalidOperationException(
                $"Can't compare expressions of type {leftExp.DataType.ToILString()} and {rightExp.DataType.ToILString()} for equality.");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = leftExp.DataType;
        if (type is OptionalType optionalType)
        {
            if (left.IsNone && right.IsNone) return true;
            if (left.IsNone || right.IsNone) return false;
            return EqualsAsync(optionalType.Referent, left, right);
        }

        return EqualsAsync(type, left, right);
    }

    private static bool EqualsAsync(DataType type, AzothValue left, AzothValue right)
    {
        if (type == DataType.Int) return left.IntValue.Equals(right.IntValue);
        if (type == DataType.UInt) return left.IntValue.Equals(right.IntValue);
        if (type == DataType.Int8) return left.I8Value.Equals(right.I8Value);
        if (type == DataType.Byte) return left.ByteValue.Equals(right.ByteValue);
        if (type == DataType.Int16) return left.I16Value.Equals(right.I16Value);
        if (type == DataType.UInt16) return left.U16Value.Equals(right.U16Value);
        if (type == DataType.Int32) return left.I32Value.Equals(right.I32Value);
        if (type == DataType.UInt32) return left.U32Value.Equals(right.U32Value);
        if (type == DataType.Int64) return left.I64Value.Equals(right.I64Value);
        if (type == DataType.UInt64) return left.U64Value.Equals(right.U64Value);
        if (type == DataType.Offset) return left.OffsetValue.Equals(right.OffsetValue);
        if (type == DataType.Size) return left.SizeValue.Equals(right.SizeValue);
        if (type is ReferenceType { IsIdentityReference: true })
            return ReferenceEquals(left.ObjectValue, right.ObjectValue);
        throw new NotImplementedException($"Compare equality of `{type.ToILString()}`.");
    }

    private async ValueTask<int> CompareAsync(IExpression leftExp, IExpression rightExp, LocalVariableScope variables)
    {
        if (leftExp.DataType != rightExp.DataType)
            throw new InvalidOperationException(
                $"Can't compare expressions of type {leftExp.DataType.ToILString()} and {rightExp.DataType.ToILString()}.");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = leftExp.DataType;
        if (type is OptionalType optionalType)
        {
            if (left.IsNone && right.IsNone) return 0;
            if (left.IsNone || right.IsNone) throw new NotImplementedException("No comparison order");
            return CompareAsync(optionalType.Referent, left, right);
        }
        return CompareAsync(type, left, right);
    }

    private static int CompareAsync(DataType type, AzothValue left, AzothValue right)
    {
        if (type == DataType.Int) return left.IntValue.CompareTo(right.IntValue);
        if (type == DataType.UInt) return left.IntValue.CompareTo(right.IntValue);
        if (type == DataType.Int8) return left.I8Value.CompareTo(right.I8Value);
        if (type == DataType.Byte) return left.ByteValue.CompareTo(right.ByteValue);
        if (type == DataType.Int16) return left.I16Value.CompareTo(right.I16Value);
        if (type == DataType.UInt16) return left.U16Value.CompareTo(right.U16Value);
        if (type == DataType.Int32) return left.I32Value.CompareTo(right.I32Value);
        if (type == DataType.UInt32) return left.U32Value.CompareTo(right.U32Value);
        if (type == DataType.Int64) return left.I64Value.CompareTo(right.I64Value);
        if (type == DataType.UInt64) return left.U64Value.CompareTo(right.U64Value);
        if (type == DataType.Offset) return left.OffsetValue.CompareTo(right.OffsetValue);
        if (type == DataType.Size) return left.SizeValue.CompareTo(right.SizeValue);
        throw new NotImplementedException($"Compare `{type.ToILString()}`.");
    }

    private async ValueTask<AzothValue> NegateAsync(IExpression expression, LocalVariableScope variables)
    {
        var value = await ExecuteAsync(expression, variables).ConfigureAwait(false);
        var dataType = expression.DataType;
        if (dataType == DataType.Int) return AzothValue.Int(-value.IntValue);
        if (dataType == DataType.Int8) return AzothValue.I8((sbyte)-value.I8Value);
        if (dataType == DataType.Int16) return AzothValue.I16((short)-value.I16Value);
        if (dataType == DataType.Int32) return AzothValue.I32(-value.I32Value);
        if (dataType == DataType.Int64) return AzothValue.I64(-value.I64Value);
        if (dataType is IntegerConstantType) return AzothValue.Int(-value.IntValue);
        throw new NotImplementedException($"Negate {dataType.ToILString()}");
    }

    private static AzothValue Remainder(
        AzothValue dividend,
        AzothValue divisor,
        NumericType type)
    {
        if (type == DataType.Int) return AzothValue.Int(dividend.IntValue % divisor.IntValue);
        if (type == DataType.UInt) return AzothValue.Int(dividend.IntValue % divisor.IntValue);
        if (type == DataType.Int8) return AzothValue.I8((sbyte)(dividend.I8Value % divisor.I8Value));
        if (type == DataType.Byte) return AzothValue.Byte((byte)(dividend.ByteValue % divisor.ByteValue));
        if (type == DataType.Int16) return AzothValue.I16((short)(dividend.I16Value % divisor.I16Value));
        if (type == DataType.UInt16) return AzothValue.U16((ushort)(dividend.U16Value % divisor.U16Value));
        if (type == DataType.Int32) return AzothValue.I32(dividend.I32Value % divisor.I32Value);
        if (type == DataType.UInt32) return AzothValue.U32(dividend.U32Value % divisor.U32Value);
        if (type == DataType.Int64) return AzothValue.I64(dividend.I64Value % divisor.I64Value);
        if (type == DataType.UInt64) return AzothValue.U64(dividend.U64Value % divisor.U64Value);
        if (type == DataType.Offset) return AzothValue.Offset(dividend.OffsetValue % divisor.OffsetValue);
        if (type == DataType.Size) return AzothValue.Size(dividend.SizeValue % divisor.SizeValue);
        throw new NotImplementedException($"Remainder {type.ToILString()}");
    }

    private async ValueTask<AzothValue> ToDisplayStringAsync(AzothValue value, NumericType type)
    {
        string displayString;
        if (type is IntegerConstantType) displayString = value.IntValue.ToString();
        else if (type == DataType.Int) displayString = value.IntValue.ToString();
        else if (type == DataType.UInt) displayString = value.IntValue.ToString();
        else if (type == DataType.Byte) displayString = value.ByteValue.ToString();
        else if (type == DataType.Int32) displayString = value.I32Value.ToString();
        else if (type == DataType.UInt32) displayString = value.U32Value.ToString();
        else if (type == DataType.Offset) displayString = value.OffsetValue.ToString();
        else if (type == DataType.Size) displayString = value.SizeValue.ToString();
        else throw new NotImplementedException($"to_display_string({type.ToILString()})");

        return await ConstructStringAsync(displayString).ConfigureAwait(false);
    }

    private async ValueTask<AzothValue> ExecuteBlockOrResultAsync(
        IBlockOrResult statement,
        LocalVariableScope variables)
    {
        return statement switch
        {
            IBlockExpression b => await ExecuteAsync(b, variables).ConfigureAwait(false),
            IResultStatement s => await ExecuteAsync(s.Expression, variables).ConfigureAwait(false),
            _ => throw ExhaustiveMatch.Failed(statement)
        };
    }

    private async ValueTask<AzothValue> ExecuteElseAsync(
        IElseClause elseClause,
        LocalVariableScope variables)
    {
        return elseClause switch
        {
            IBlockOrResult exp => await ExecuteBlockOrResultAsync(exp, variables).ConfigureAwait(false),
            IIfExpression exp => await ExecuteAsync(exp, variables).ConfigureAwait(false),
            _ => throw ExhaustiveMatch.Failed(elseClause)
        };
    }

    private async ValueTask ExecuteAssignmentAsync(
        IAssignableExpression expression,
        AzothValue value,
        LocalVariableScope variables)
    {
        switch (expression)
        {
            default:
                throw new NotImplementedException($"Can't interpret assignment into {expression.GetType().Name}");
            case IVariableNameExpression exp:
                variables[exp.ReferencedSymbol] = value;
                break;
            case IFieldAccessExpression exp:
                var obj = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                // TODO handle the access operator
                obj.ObjectValue[exp.ReferencedSymbol.Name] = value;
                break;
        }
    }

    public Task WaitForExitAsync() => executionTask;

    public TextReader StandardOutput { get; }
    public TextReader StandardError => TextReader.Null;

    public byte ExitCode => exitCode ?? throw new InvalidOperationException("Process has not exited");
}
