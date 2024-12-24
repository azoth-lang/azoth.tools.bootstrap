using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Async;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout.BoundedLists;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

public class InterpreterProcess
{
    public static InterpreterProcess StartEntryPoint(IPackageNode package, IEnumerable<IPackageNode> referencedPackages)
    {
        if (package.EntryPoint is null)
            throw new ArgumentException("Cannot execute package without an entry point");

        return new InterpreterProcess(package, referencedPackages, runTests: false);
    }

    public static InterpreterProcess StartTests(IPackageNode package, IEnumerable<IPackageNode> referencedPackages)
        => new(package, referencedPackages, runTests: true);

    private readonly IPackageNode package;
    private readonly Task executionTask;
    private readonly FixedDictionary<FunctionSymbol, IConcreteFunctionInvocableDefinitionNode> functions;
    private readonly FixedDictionary<MethodSymbol, IMethodDefinitionNode> structMethods;
    private readonly FixedDictionary<ConstructorSymbol, IOrdinaryConstructorDefinitionNode?> constructors;
    private readonly FixedDictionary<InitializerSymbol, IOrdinaryInitializerDefinitionNode?> initializers;
    private readonly FixedDictionary<OrdinaryTypeSymbol, ITypeDefinitionNode> userTypes;
    private readonly IClassDefinitionNode stringClass;
    private readonly IOrdinaryConstructorDefinitionNode stringConstructor;
    private readonly IStructDefinitionNode? rangeStruct;
    private readonly InitializerSymbol? rangeInitializer;
    private byte? exitCode;
    private readonly MemoryStream standardOutput = new();
    private readonly TextWriter standardOutputWriter;
    private readonly MethodSignatureCache methodSignatures = new();
    private readonly ConcurrentDictionary<IClassDefinitionNode, VTable> vTables = new();

    private InterpreterProcess(IPackageNode package, IEnumerable<IPackageNode> referencedPackages, bool runTests)
    {
        this.package = package;
        var allDefinitions = GetAllDefinitions(package, referencedPackages,
            runTests ? r => r.MainFacet.Definitions.Concat(r.TestingFacet.Definitions) : r => r.MainFacet.Definitions);
        functions = allDefinitions
                    .OfType<IConcreteFunctionInvocableDefinitionNode>()
                    .ToFixedDictionary(f => f.Symbol.Assigned());

        structMethods = allDefinitions
                        .OfType<IMethodDefinitionNode>()
                        .Where(m => m.Symbol.Assigned().ContextTypeSymbol is OrdinaryTypeSymbol { Kind: TypeKind.Struct })
                        .ToFixedDictionary(m => m.Symbol.Assigned());

        userTypes = allDefinitions.OfType<ITypeDefinitionNode>()
                                 .ToFixedDictionary(c => c.Symbol);
        stringClass = userTypes.Values.OfType<IClassDefinitionNode>().Single(c => c.Symbol.Name == "String");
        stringConstructor = stringClass.Members.OfType<IOrdinaryConstructorDefinitionNode>().Single(c => c.Parameters.Count == 3);
        rangeStruct = userTypes.Values.OfType<IStructDefinitionNode>().SingleOrDefault(c => c.Symbol.Name == "range");
        rangeInitializer = rangeStruct?.Members.OfType<IInitializerDefinitionNode>().SingleOrDefault(c => c.Parameters.Count == 2)?.Symbol;
        var defaultConstructorSymbols = allDefinitions
                                        .OfType<IClassDefinitionNode>()
                                        .Select(c => c.DefaultConstructor?.Symbol).WhereNotNull();
        constructors = defaultConstructorSymbols
                       .Select(c => (c, default(IOrdinaryConstructorDefinitionNode)))
                       .Concat(allDefinitions
                               .OfType<IOrdinaryConstructorDefinitionNode>()
                               .Select(c => (c.Symbol.Assigned(), (IOrdinaryConstructorDefinitionNode?)c)))
                       .ToFixedDictionary();

        var defaultInitializerSymbols = allDefinitions
                                       .OfType<IStructDefinitionNode>()
                                       .Select(c => c.DefaultInitializer?.Symbol).WhereNotNull();
        initializers = defaultInitializerSymbols
                       .Select(c => (c, default(IOrdinaryInitializerDefinitionNode)))
                       .Concat(allDefinitions
                               .OfType<IOrdinaryInitializerDefinitionNode>()
                               .Select(c => (c.Symbol.Assigned(), (IOrdinaryInitializerDefinitionNode?)c)))
                       .ToFixedDictionary();

        // TODO pointing both of these to a memory stream is probably wrong. Need something that acts like a pipe.
        standardOutputWriter = new StreamWriter(standardOutput, Encoding.UTF8, leaveOpen: true);
        StandardOutput = new StreamReader(standardOutput, Encoding.UTF8);

        executionTask = runTests ? Task.Run(RunTestsAsync) : Task.Run(CallEntryPointAsync);
    }

    private static IFixedList<IDefinitionNode> GetAllDefinitions(
        IPackageNode package,
        IEnumerable<IPackageNode> referencedPackages,
        Func<IPackageNode, IEnumerable<IFacetMemberDefinitionNode>> getPackageMemberDefinitions)
        => GetAllDefinitions(referencedPackages.Prepend(package).SelectMany(getPackageMemberDefinitions)).ToFixedList();

    private static IEnumerable<IDefinitionNode> GetAllDefinitions(
        IEnumerable<IFacetMemberDefinitionNode> packageMemberDefinitions)
    {
        var definitions = new Queue<IDefinitionNode>();
        definitions.EnqueueRange(packageMemberDefinitions);
        while (definitions.TryDequeue(out var definition))
        {
            yield return definition;
            if (definition is ITypeDefinitionNode syn)
                definitions.EnqueueRange(syn.Members);
        }
    }

    private async Task CallEntryPointAsync()
    {
        try
        {
            var entryPoint = package.EntryPoint!;
            var arguments = new List<AzothValue>();
            foreach (var parameterType in entryPoint.Symbol.Assigned().ParameterTypes)
                arguments.Add(await ConstructMainParameterAsync(parameterType.Type));

            var returnValue = await CallFunctionAsync(entryPoint, arguments).ConfigureAwait(false);
            // Flush any buffered output
            await standardOutputWriter.FlushAsync().ConfigureAwait(false);
            var returnType = entryPoint.Symbol.Assigned().ReturnType;
            if (returnType.Equals(Type.Void))
                exitCode = 0;
            else if (returnType.Equals(Type.Byte))
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
            var testFunctions = package.TestingFacet.Definitions.OfType<IFunctionDefinitionNode>()
                                       .Where(f => f.Attributes.Any(IsTestAttribute)).ToFixedSet();

            await standardOutputWriter.WriteLineAsync($"Testing {package.Symbol.Name} package...");
            await standardOutputWriter.WriteLineAsync($"  Found {testFunctions.Count} tests");
            await standardOutputWriter.WriteLineAsync();

            int failed = 0;

            foreach (var function in testFunctions)
            {
                // TODO check that return type is void
                var symbol = function.Symbol;
                await standardOutputWriter.WriteLineAsync($"{symbol.Assigned().ContainingSymbol.ToILString()}.{symbol.Assigned().Name} ...");
                try
                {
                    await CallFunctionAsync(function, []).ConfigureAwait(false);
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

    private static bool IsTestAttribute(IAttributeNode attribute)
        => attribute.TypeName.ReferencedDeclaration!.Name.Text == "Test_Attribute";

    private async Task<AzothValue> ConstructMainParameterAsync(Type parameterType)
    {
        if (parameterType is not CapabilityType { Arguments.Count: 0 } type)
            throw new InvalidOperationException(
                $"Parameter to main of type {parameterType.ToILString()} not supported");

        // TODO further restrict what can be passed to main

        var @class = userTypes.Values.OfType<IClassDefinitionNode>().Single(c => c.Symbol.TypeConstructor.Equals(type.TypeConstructor));
        var constructorSymbol = @class.DefaultConstructor?.Symbol.Assigned()
            ?? @class.Members.OfType<IConstructorDefinitionNode>().Select(c => c.Symbol.Assigned())
                     .Single(c => c.Arity == 0);
        return await ConstructClass(@class, constructorSymbol, []);
    }

    internal async Task<AzothValue> CallFunctionAsync(
        IConcreteFunctionInvocableDefinitionNode function,
        IEnumerable<AzothValue> arguments)
    {
        try
        {
            var variables = new LocalVariableScope();
            foreach (var (arg, parameter) in arguments.EquiZip(function.Parameters))
                variables.Add(parameter, arg);

            return await ExecuteAsync(function.Body.Statements, variables).ConfigureAwait(false);
        }
        catch (Return @return)
        {
            return @return.Value;
        }
    }

    private async Task<AzothValue> ConstructClass(
        IClassDefinitionNode @class,
        ConstructorSymbol constructorSymbol,
        IEnumerable<AzothValue> arguments)
    {
        var vTable = vTables.GetOrAdd(@class, CreateVTable);
        var self = AzothValue.Object(new AzothObject(vTable));
        return await CallConstructorAsync(@class, constructorSymbol, self, arguments);
    }

    private async Task<AzothValue> CallConstructorAsync(
        IClassDefinitionNode @class,
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
        IOrdinaryConstructorDefinitionNode constructor,
        AzothValue self,
        IEnumerable<AzothValue> arguments)
    {
        try
        {
            var variables = new LocalVariableScope();
            variables.Add(constructor.SelfParameter, self);
            foreach (var (arg, parameter) in arguments.EquiZip(constructor.Parameters))
                switch (parameter)
                {
                    default:
                        throw ExhaustiveMatch.Failed(parameter);
                    case IFieldParameterNode fieldParameter:
                        self.ObjectValue[fieldParameter.ReferencedField!.Symbol.Assigned().Name] = arg;
                        break;
                    case INamedParameterNode p:
                        variables.Add(p, arg);
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
    private async ValueTask<AzothValue> CallDefaultConstructorAsync(IClassDefinitionNode @class, AzothValue self)
    {
        // Initialize fields to default values
        var fields = @class.Members.OfType<IFieldDefinitionNode>();
        foreach (var field in fields)
            self.ObjectValue[field.Symbol.Assigned().Name] = new AzothValue();

        if (@class.BaseTypeName?.ReferencedDeclaration!.Symbol is OrdinaryTypeSymbol baseClassSymbol)
        {
            var baseClass = (IClassDefinitionNode)userTypes[baseClassSymbol];
            var noArgConstructorSymbol = NoArgConstructorSymbol(baseClass);
            await CallConstructorAsync(baseClass, noArgConstructorSymbol, self, []);
        }

        return self;
    }

    private static ConstructorSymbol NoArgConstructorSymbol(IClassDefinitionNode baseClass)
    {
        return baseClass.DefaultConstructor?.Symbol
               ?? baseClass.Members.OfType<IOrdinaryConstructorDefinitionNode>().Select(c => c.Symbol.Assigned())
                           .Single(c => c.Arity == 0);
    }

    private async Task<AzothValue> InitializeStruct(
        IStructDefinitionNode @struct,
        InitializerSymbol initializerSymbol,
        IEnumerable<AzothValue> arguments)
    {
        var self = AzothValue.Struct(new AzothStruct());
        return await CallInitializerAsync(@struct, initializerSymbol, self, arguments).ConfigureAwait(false);
    }

    private async Task<AzothValue> CallInitializerAsync(
        IStructDefinitionNode @struct,
        InitializerSymbol initializerSymbol,
        AzothValue self,
        IEnumerable<AzothValue> arguments)
    {
        // TODO run field initializers
        var initializer = initializers[initializerSymbol];
        // Default constructor is null
        if (initializer is null) return await CallDefaultInitializerAsync(@struct, self);
        return await CallInitializerAsync(initializer, self, arguments).ConfigureAwait(false);
    }

    private async ValueTask<AzothValue> CallInitializerAsync(
        IOrdinaryInitializerDefinitionNode initializer,
        AzothValue self,
        IEnumerable<AzothValue> arguments)
    {
        try
        {
            var variables = new LocalVariableScope();
            variables.Add(initializer.SelfParameter, self);
            foreach (var (arg, parameter) in arguments.EquiZip(initializer.Parameters))
                switch (parameter)
                {
                    default:
                        throw ExhaustiveMatch.Failed(parameter);
                    case IFieldParameterNode fieldParameter:
                        self.ObjectValue[fieldParameter.ReferencedField!.Symbol.Assigned().Name] = arg;
                        break;
                    case INamedParameterNode p:
                        variables.Add(p, arg);
                        break;
                }

            foreach (var statement in initializer.Body.Statements)
                await ExecuteAsync(statement, variables).ConfigureAwait(false);
            return self;
        }
        catch (Return)
        {
            return self;
        }
    }

    /// <summary>
    /// Call the implicit default initializer for a type that has no constructors.
    /// </summary>
    private static ValueTask<AzothValue> CallDefaultInitializerAsync(IStructDefinitionNode @struct, AzothValue self)
    {
        // Initialize fields to default values
        var fields = @struct.Members.OfType<IFieldDefinitionNode>();
        foreach (var field in fields)
            self.ObjectValue[field.Symbol.Assigned().Name] = new AzothValue();

        return ValueTask.FromResult(self);
    }

    private async ValueTask<AzothValue> CallMethodAsync(
        MethodSymbol methodSymbol,
        Type selfType,
        AzothValue self,
        IEnumerable<AzothValue> arguments)
    {
        switch (selfType)
        {
            case VoidType _:
            case NeverType _:
            case OptionalType _:
            case GenericParameterType _:
            case FunctionType _:
            case CapabilitySetSelfType _:
            case CapabilityViewpointType _:
            case SelfViewpointType _:
                var methodSignature = methodSignatures[methodSymbol];
                throw new InvalidOperationException($"Can't call {methodSignature} on {selfType}");
            case CapabilityType capabilityType:
                return await CallMethodAsync(methodSymbol, capabilityType, self, arguments);
            default:
                throw ExhaustiveMatch.Failed(selfType);
        }
    }

    private async ValueTask<AzothValue> CallMethodAsync(
        MethodSymbol methodSymbol,
        CapabilityType selfType,
        AzothValue self,
        IEnumerable<AzothValue> arguments)
    {
        var referenceCall = selfType.TypeConstructor.Semantics switch
        {
            // TODO this is an odd case, generic instantiation should avoid it but this works for now
            null => self.IsObject,
            TypeSemantics.Value => false,
            TypeSemantics.Reference => true,
            _ => throw ExhaustiveMatch.Failed(selfType.TypeConstructor.Semantics),
        };

        return referenceCall
            ? await CallClassMethodAsync(methodSymbol, self, arguments)
            : await CallStructMethod(methodSymbol, selfType, self, arguments);
    }

    private async ValueTask<AzothValue> CallClassMethodAsync(
        MethodSymbol methodSymbol,
        AzothValue self,
        IEnumerable<AzothValue> arguments)
    {
        var methodSignature = methodSignatures[methodSymbol];
        var vtable = self.ObjectValue.VTable;
        var method = vtable[methodSignature];
        return await CallMethodAsync(method, self, arguments).ConfigureAwait(false);
    }

    private async ValueTask<AzothValue> CallStructMethod(
        MethodSymbol methodSymbol,
        CapabilityType selfType,
        AzothValue self,
        IEnumerable<AzothValue> arguments)
    {
        return methodSymbol.Name.Text switch
        {
            "remainder" => Remainder(self, arguments.Single(), selfType),
            "to_display_string" => await ToDisplayStringAsync(self, selfType),
            _ => await CallMethodAsync(structMethods[methodSymbol], self, arguments),
        };
    }

    private async ValueTask<AzothValue> CallMethodAsync(
        IMethodDefinitionNode method,
        AzothValue self,
        IEnumerable<AzothValue> arguments)
    {
        if (method.Body is null)
            throw new InvalidOperationException($"Can't call abstract method {method}");

        try
        {
            var variables = new LocalVariableScope();
            variables.Add(method.SelfParameter, self);
            foreach (var (arg, parameter) in arguments.EquiZip(method.Parameters))
                variables.Add(parameter, arg);

            return await ExecuteAsync(method.Body.Statements, variables).ConfigureAwait(false);
        }
        catch (Return @return)
        {
            return @return.Value;
        }
    }

    private async Task<AzothValue> ExecuteAsync(IFixedList<IStatementNode> statements, LocalVariableScope variables)
    {
        foreach (var statement in statements)
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IResultStatementNode resultStatement:
                    return await ExecuteAsync(resultStatement.Expression!, variables).ConfigureAwait(false);
                case IBodyStatementNode bodyStatement:
                    await ExecuteAsync(bodyStatement, variables).ConfigureAwait(false);
                    break;
            }

        return AzothValue.None;
    }

    private async ValueTask ExecuteAsync(IBodyStatementNode statement, LocalVariableScope variables)
    {
        switch (statement)
        {
            default:
                throw ExhaustiveMatch.Failed(statement);
            case IExpressionStatementNode s:
                await ExecuteAsync(s.Expression!, variables).ConfigureAwait(false);
                break;
            case IVariableDeclarationStatementNode d:
            {
                var initialValue = d.Initializer is null
                    ? AzothValue.None
                    : await ExecuteAsync(d.Initializer, variables).ConfigureAwait(false);
                variables.Add(d, initialValue);
                break;
            }
        }
    }

    private ValueTask<AzothValue> ExecuteAsync(IResultStatementNode statement, LocalVariableScope variables)
        => ExecuteAsync(statement.Expression!, variables);

    private async ValueTask<AzothValue> ExecuteAsync(IExpressionNode expression, LocalVariableScope variables)
    {
        switch (expression)
        {
            default:
                throw ExhaustiveMatch.Failed(expression);
            case IMoveExpressionNode exp:
                return await ExecuteAsync(exp.Referent, variables);
            case IImplicitTempMoveExpressionNode exp:
                return await ExecuteAsync(exp.Referent, variables);
            case IFreezeExpressionNode exp:
                return await ExecuteAsync(exp.Referent, variables);
            case INoneLiteralExpressionNode _:
                return AzothValue.None;
            case IReturnExpressionNode exp:
                if (exp.Value is null) throw new Return();
                throw new Return(await ExecuteAsync(exp.Value, variables).ConfigureAwait(false));
            case IConversionExpressionNode exp:
            {
                var value = await ExecuteAsync(exp.Referent!, variables).ConfigureAwait(false);
                return value.Convert(exp.Referent!.Type.Known(), (CapabilityType)exp.ConvertToType.NamedType, false);
            }
            case IImplicitConversionExpressionNode exp:
            {
                var value = await ExecuteAsync(exp.Referent, variables).ConfigureAwait(false);
                return value.Convert(exp.Referent.Type.Known(), (CapabilityType)exp.Type, true);
            }
            case IIntegerLiteralExpressionNode exp:
                return AzothValue.Int(exp.Value);
            case IFunctionInvocationExpressionNode exp:
            {
                var arguments = await ExecuteArgumentsAsync(exp.Arguments!, variables).ConfigureAwait(false);
                var functionSymbol = exp.Function.ReferencedDeclaration!.Symbol.Assigned();
                if (functionSymbol.Package == Intrinsic.SymbolTree.Package)
                    return await CallIntrinsicAsync(functionSymbol, arguments).ConfigureAwait(false);
                return await CallFunctionAsync(functions[functionSymbol], arguments).ConfigureAwait(false);
            }
            case IFunctionReferenceInvocationExpressionNode exp:
            {
                var function = await ExecuteAsync(exp.Expression, variables).ConfigureAwait(false);
                var arguments = await ExecuteArgumentsAsync(exp.Arguments!, variables).ConfigureAwait(false);
                return await function.FunctionReferenceValue.CallAsync(arguments).ConfigureAwait(false);
            }
            case IInitializerInvocationExpressionNode exp:
            {
                var arguments = await ExecuteArgumentsAsync(exp.Arguments!, variables).ConfigureAwait(false);
                var initializerSymbol = exp.ReferencedDeclaration!.Symbol.Assigned();
                var @struct = (IStructDefinitionNode)userTypes[initializerSymbol.ContextTypeSymbol];
                return await InitializeStruct(@struct, initializerSymbol, arguments).ConfigureAwait(false);
            }
            case IBoolLiteralExpressionNode exp:
                return AzothValue.Bool(exp.Value);
            case IIfExpressionNode exp:
            {
                var condition = await ExecuteAsync(exp.Condition!, variables).ConfigureAwait(false);
                if (condition.BoolValue)
                    return await ExecuteBlockOrResultAsync(exp.ThenBlock, variables).ConfigureAwait(false);
                if (exp.ElseClause is not null)
                    return await ExecuteElseAsync(exp.ElseClause, variables).ConfigureAwait(false);
                return AzothValue.None;
            }
            case IVariableNameExpressionNode exp:
                return variables[exp.ReferencedDefinition];
            case IFunctionNameNode exp:
                return AzothValue.FunctionReference(new ConcreteFunctionReference(this, functions[exp.ReferencedDeclaration!.Symbol.Assigned()]));
            case IBlockExpressionNode block:
            {
                var blockVariables = new LocalVariableScope(variables);
                return await ExecuteAsync(block.Statements, blockVariables);
            }
            case ILoopExpressionNode exp:
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
            case IWhileExpressionNode exp:
                try
                {
                    for (; ; )
                    {
                        // Create a variable scope in case a variable is declared by a pattern in the condition
                        var loopVariables = new LocalVariableScope(variables);
                        var condition = await ExecuteAsync(exp.Condition!, loopVariables).ConfigureAwait(false);
                        if (!condition.BoolValue)
                            return AzothValue.None;
                        try
                        {
                            await ExecuteAsync(exp.Block, loopVariables).ConfigureAwait(false);
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
            case IBreakExpressionNode exp:
                if (exp.Value is null) throw new Break();
                throw new Break(await ExecuteAsync(exp.Value, variables).ConfigureAwait(false));
            case INextExpressionNode _:
                throw new Next();
            case IAssignmentExpressionNode exp:
            {
                // TODO this evaluates the left hand side twice for compound operators
                var value = exp.Operator switch
                {
                    AssignmentOperator.Simple =>
                        // TODO the expression being assigned into is supposed to be evaluated first
                        await ExecuteAsync(exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Plus
                        => await AddAsync(exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Minus
                        => await SubtractAsync(exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Asterisk
                        => await MultiplyAsync(exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false),
                    AssignmentOperator.Slash
                        => await DivideAsync(exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false),
                    _ => throw ExhaustiveMatch.Failed(exp.Operator)
                };
                await ExecuteAssignmentAsync(exp.LeftOperand!, value, variables).ConfigureAwait(false);
                return value;
            }
            case IBinaryOperatorExpressionNode exp:
                switch (exp.Operator)
                {
                    default:
                        throw ExhaustiveMatch.Failed();
                    case BinaryOperator.Plus:
                        return await AddAsync(exp.LeftOperand!, exp.RightOperand!, variables).ConfigureAwait(false);
                    case BinaryOperator.Minus:
                        return await SubtractAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.Asterisk:
                        return await MultiplyAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.Slash:
                        return await DivideAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false);
                    case BinaryOperator.EqualsEquals:
                        return AzothValue.Bool(await BuiltInEqualsAsync(exp.NumericOperatorCommonPlainType!, exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false));
                    case BinaryOperator.NotEqual:
                        return AzothValue.Bool(!await BuiltInEqualsAsync(exp.NumericOperatorCommonPlainType!, exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false));
                    case BinaryOperator.ReferenceEquals:
                        return AzothValue.Bool(await ReferenceEqualsAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false));
                    case BinaryOperator.NotReferenceEqual:
                        return AzothValue.Bool(!await ReferenceEqualsAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false));
                    case BinaryOperator.LessThan:
                        return AzothValue.Bool(await CompareAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false) < 0);
                    case BinaryOperator.LessThanOrEqual:
                        return AzothValue.Bool(await CompareAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false) <= 0);
                    case BinaryOperator.GreaterThan:
                        return AzothValue.Bool(await CompareAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false) > 0);
                    case BinaryOperator.GreaterThanOrEqual:
                        return AzothValue.Bool(await CompareAsync(exp.LeftOperand!, exp.RightOperand!, variables)
                            .ConfigureAwait(false) >= 0);
                    case BinaryOperator.And:
                    {
                        var left = await ExecuteAsync(exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (!left.BoolValue) return AzothValue.Bool(false);
                        return await ExecuteAsync(exp.RightOperand!, variables).ConfigureAwait(false);
                    }
                    case BinaryOperator.Or:
                    {
                        var left = await ExecuteAsync(exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (left.BoolValue) return AzothValue.Bool(true);
                        return await ExecuteAsync(exp.RightOperand!, variables).ConfigureAwait(false);
                    }
                    case BinaryOperator.DotDot:
                    case BinaryOperator.LessThanDotDot:
                    case BinaryOperator.DotDotLessThan:
                    case BinaryOperator.LessThanDotDotLessThan:
                    {
                        var left = await ExecuteAsync(exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (!exp.Operator.RangeInclusiveOfStart()) left = left.Increment(Type.Int);
                        var right = await ExecuteAsync(exp.RightOperand!, variables).ConfigureAwait(false);
                        if (exp.Operator.RangeInclusiveOfEnd()) right = right.Increment(Type.Int);
                        return await InitializeStruct(rangeStruct!, rangeInitializer!, [left, right]);
                    }
                    case BinaryOperator.QuestionQuestion:
                    {
                        var left = await ExecuteAsync(exp.LeftOperand!, variables).ConfigureAwait(false);
                        if (!left.IsNone) return left;
                        return await ExecuteAsync(exp.RightOperand!, variables).ConfigureAwait(false);
                    }
                }
            case IUnaryOperatorExpressionNode exp:
                return exp.Operator switch
                {
                    UnaryOperator.Not => AzothValue.Bool(
                        !(await ExecuteAsync(exp.Operand!, variables).ConfigureAwait(false)).BoolValue),
                    UnaryOperator.Minus => await NegateAsync(exp.Operand!, variables).ConfigureAwait(false),
                    UnaryOperator.Plus => await ExecuteAsync(exp.Operand!, variables).ConfigureAwait(false),
                    _ => throw ExhaustiveMatch.Failed(exp.Operator)
                };
            case IMethodInvocationExpressionNode exp:
            {
                var self = await ExecuteAsync(exp.Method.Context, variables).ConfigureAwait(false);
                var arguments = await ExecuteArgumentsAsync(exp.Arguments!, variables).ConfigureAwait(false);
                var methodSymbol = exp.Method.ReferencedDeclaration!.Symbol.Assigned();
                if (methodSymbol.Package == Intrinsic.SymbolTree.Package)
                    return await CallIntrinsicAsync(methodSymbol, self, arguments);
                if (methodSymbol == Primitive.IdentityHash)
                    return IdentityHash(self);

                var selfType = exp.Method.Context.Type.Known();
                return await CallMethodAsync(methodSymbol, selfType, self, arguments);
            }
            case IGetterInvocationExpressionNode exp:
            {
                var self = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                var getterSymbol = exp.ReferencedDeclaration!.Symbol.Assigned();
                if (getterSymbol.Package == Intrinsic.SymbolTree.Package)
                    return await CallIntrinsicAsync(getterSymbol, self, []);
                var selfType = exp.Context.Type.Known();
                return await CallMethodAsync(getterSymbol, selfType, self, []).ConfigureAwait(false);
            }
            case ISetterInvocationExpressionNode exp:
            {
                var self = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                var value = await ExecuteAsync(exp.Value!, variables).ConfigureAwait(false);
                var setterSymbol = exp.ReferencedDeclaration!.Symbol.Assigned();
                if (setterSymbol.Package == Intrinsic.SymbolTree.Package)
                    return await CallIntrinsicAsync(setterSymbol, self, [value]);
                var selfType = exp.Context.Type.Known();
                return await CallMethodAsync(setterSymbol, selfType, self, [value]);
            }
            case INewObjectExpressionNode exp:
            {
                var arguments = await ExecuteArgumentsAsync(exp.Arguments!, variables).ConfigureAwait(false);
                var constructorSymbol = exp.ReferencedConstructor!.Symbol.Assigned();
                var objectTypeSymbol = constructorSymbol.ContainingSymbol;
                if (objectTypeSymbol.Package == Intrinsic.SymbolTree.Package)
                    return await CallIntrinsicAsync(constructorSymbol, arguments).ConfigureAwait(false);
                var @class = (IClassDefinitionNode)userTypes[objectTypeSymbol];
                return await ConstructClass(@class, constructorSymbol, arguments);
            }
            case IPrepareToReturnExpressionNode exp:
                return await ExecuteAsync(exp.Value, variables).ConfigureAwait(false);
            case ISelfExpressionNode exp:
                return variables[exp.ReferencedDefinition!];
            case IStringLiteralExpressionNode exp:
            {
                // Call the constructor of the string class
                var value = exp.Value;
                return await ConstructStringAsync(value);
            }
            case IUnsafeExpressionNode exp:
                return await ExecuteAsync(exp.Expression!, variables).ConfigureAwait(false);
            case IFieldAccessExpressionNode exp:
            {
                var obj = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                return obj.ObjectValue[exp.ReferencedDeclaration.Name];
            }
            case IForeachExpressionNode exp:
            {
                var iterable = await ExecuteAsync(exp.InExpression!, variables).ConfigureAwait(false);
                IBindingNode loopVariable = exp;
                // Call `iterable.iterate()` if it exists
                AzothValue iterator;
                CapabilityType iteratorType;
                if (exp.ReferencedIterateMethod is not null)
                {
                    var selfType = (CapabilityType)exp.InExpression!.Type;
                    var iterateMethod = exp.ReferencedIterateMethod!.Symbol.Assigned();
                    iterator = await CallMethodAsync(iterateMethod, selfType, iterable, []).ConfigureAwait(false);
                    iteratorType = (CapabilityType)iterateMethod.ReturnType;
                }
                else
                {
                    iterator = iterable;
                    iteratorType = (CapabilityType)exp.InExpression!.Type;
                }

                try
                {
                    var nextMethod = exp.ReferencedNextMethod!.Symbol.Assigned();
                    while (true)
                    {
                        var value = await CallMethodAsync(nextMethod, iteratorType, iterator, []).ConfigureAwait(false);
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
            case IPatternMatchExpressionNode exp:
            {
                var value = await ExecuteAsync(exp.Referent!, variables).ConfigureAwait(false);
                return await ExecuteMatchAsync(value, exp.Pattern, variables);
            }
            case IAsyncBlockExpressionNode exp:
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
            case IAsyncStartExpressionNode exp:
            {
                if (variables.AsyncScope is not AsyncScope asyncScope)
                    throw new InvalidOperationException("Cannot execute `go` or `do` expression outside of an async scope.");

                var task = exp.Scheduled
                    ? Task.Run(async () => await ExecuteAsync(exp.Expression!, variables))
                    : ExecuteAsync(exp.Expression!, variables).AsTask();

                asyncScope.Add(task);

                return AzothValue.Promise(task);
            }
            case IAwaitExpressionNode exp:
            {
                var value = await ExecuteAsync(exp.Expression!, variables).ConfigureAwait(false);

                return await value.PromiseValue.ConfigureAwait(false);
            }
            case IUnknownInvocationExpressionNode _:
            case IUnknownNameExpressionNode _:
            case IMissingNameExpressionNode _:
                throw new UnreachableException($"Node type {expression.GetType().GetFriendlyName()} won't be in final tree.");
            case INameExpressionNode _:
                throw new UnreachableException($"Name node type {expression.GetType().GetFriendlyName()} won't be traversed.");
        }
    }

    private static async ValueTask<AzothValue> ExecuteMatchAsync(
        AzothValue value,
        IPatternNode pattern,
        LocalVariableScope variables)
    {
        switch (pattern)
        {
            default:
                throw ExhaustiveMatch.Failed(pattern);
            case IBindingContextPatternNode pat:
                return await ExecuteMatchAsync(value, pat.Pattern, variables).ConfigureAwait(false);
            case IBindingPatternNode pat:
                variables.Add(pat, value);
                return AzothValue.Bool(true);
            case IOptionalPatternNode pat:
                if (value.IsNone)
                    return AzothValue.Bool(false);
                return await ExecuteMatchAsync(value, pat.Pattern, variables).ConfigureAwait(false);
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
            var listType = constructor.ContainingSymbol.TypeConstructor.ParameterTypes[0];
            nuint capacity = arguments[0].SizeValue;
            IRawBoundedList list;
            if (listType.Equals(Type.Byte))
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

    private VTable CreateVTable(IClassDefinitionNode @class)
        => new(@class, methodSignatures, userTypes);

    private async ValueTask<List<AzothValue>> ExecuteArgumentsAsync(IFixedList<IExpressionNode> arguments, LocalVariableScope variables)
    {
        var values = new List<AzothValue>(arguments.Count);
        // Execute arguments in order
        foreach (var argument in arguments)
            values.Add(await ExecuteAsync(argument, variables).ConfigureAwait(false));
        return values;
    }

    private async ValueTask<AzothValue> AddAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariableScope variables)
    {
        if (!leftExp.Type.Equals(rightExp.Type))
            throw new InvalidOperationException(
                $"Can't add expressions of type {leftExp.Type.ToILString()} and {rightExp.Type.ToILString()}");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = leftExp.Type;
        if (type.Equals(Type.Int)) return AzothValue.Int(left.IntValue + right.IntValue);
        if (type.Equals(Type.UInt)) return AzothValue.Int(left.IntValue + right.IntValue);
        if (type.Equals(Type.Int8)) return AzothValue.I8((sbyte)(left.I8Value + right.I8Value));
        if (type.Equals(Type.Byte)) return AzothValue.Byte((byte)(left.ByteValue + right.ByteValue));
        if (type.Equals(Type.Int16)) return AzothValue.I16((short)(left.I16Value + right.I16Value));
        if (type.Equals(Type.UInt16)) return AzothValue.U16((ushort)(left.U16Value + right.U16Value));
        if (type.Equals(Type.Int32)) return AzothValue.I32(left.I32Value + right.I32Value);
        if (type.Equals(Type.UInt32)) return AzothValue.U32(left.U32Value + right.U32Value);
        if (type.Equals(Type.Int64)) return AzothValue.I64(left.I64Value + right.I64Value);
        if (type.Equals(Type.UInt64)) return AzothValue.U64(left.U64Value + right.U64Value);
        if (type.Equals(Type.Offset)) return AzothValue.Offset(left.OffsetValue + right.OffsetValue);
        if (type.Equals(Type.Size)) return AzothValue.Size(left.SizeValue + right.SizeValue);
        if (type.Equals(Type.NInt)) return AzothValue.NInt(left.NIntValue + right.NIntValue);
        if (type.Equals(Type.NUInt)) return AzothValue.NUInt(left.NUIntValue + right.NUIntValue);
        throw new NotImplementedException($"Add {type.ToILString()}");
    }

    private async ValueTask<AzothValue> SubtractAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariableScope variables)
    {
        // TODO check for negative values when subtracting unsigned
        if (!leftExp.Type.Equals(rightExp.Type))
            throw new InvalidOperationException(
                $"Can't subtract expressions of type {leftExp.Type} and {rightExp.Type}");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = leftExp.Type;
        if (type.Equals(Type.Int)) return AzothValue.Int(left.IntValue - right.IntValue);
        if (type.Equals(Type.UInt)) return AzothValue.Int(left.IntValue - right.IntValue);
        if (type.Equals(Type.Int8)) return AzothValue.I8((sbyte)(left.I8Value - right.I8Value));
        if (type.Equals(Type.Byte)) return AzothValue.Byte((byte)(left.ByteValue - right.ByteValue));
        if (type.Equals(Type.Int16)) return AzothValue.I16((short)(left.I16Value - right.I16Value));
        if (type.Equals(Type.UInt16)) return AzothValue.U16((ushort)(left.U16Value - right.U16Value));
        if (type.Equals(Type.Int32)) return AzothValue.I32(left.I32Value - right.I32Value);
        if (type.Equals(Type.UInt32)) return AzothValue.U32(left.U32Value - right.U32Value);
        if (type.Equals(Type.Int64)) return AzothValue.I64(left.I64Value - right.I64Value);
        if (type.Equals(Type.UInt64)) return AzothValue.U64(left.U64Value - right.U64Value);
        if (type.Equals(Type.Offset)) return AzothValue.Offset(left.OffsetValue - right.OffsetValue);
        if (type.Equals(Type.Size)) return AzothValue.Size(left.SizeValue - right.SizeValue);
        if (type.Equals(Type.NInt)) return AzothValue.NInt(left.NIntValue - right.NIntValue);
        if (type.Equals(Type.NUInt)) return AzothValue.NUInt(left.NUIntValue - right.NUIntValue);
        throw new NotImplementedException($"Subtract {type.ToILString()}");
    }

    private async ValueTask<AzothValue> MultiplyAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariableScope variables)
    {
        if (!leftExp.Type.Equals(rightExp.Type))
            throw new InvalidOperationException(
                $"Can't multiply expressions of type {leftExp.Type.ToILString()} and {rightExp.Type.ToILString()}");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = leftExp.Type;
        if (type.Equals(Type.Int)) return AzothValue.Int(left.IntValue * right.IntValue);
        if (type.Equals(Type.UInt)) return AzothValue.Int(left.IntValue * right.IntValue);
        if (type.Equals(Type.Int8)) return AzothValue.I8((sbyte)(left.I8Value * right.I8Value));
        if (type.Equals(Type.Byte)) return AzothValue.Byte((byte)(left.ByteValue * right.ByteValue));
        if (type.Equals(Type.Int16)) return AzothValue.I16((short)(left.I16Value * right.I16Value));
        if (type.Equals(Type.UInt16)) return AzothValue.U16((ushort)(left.U16Value * right.U16Value));
        if (type.Equals(Type.Int32)) return AzothValue.I32(left.I32Value * right.I32Value);
        if (type.Equals(Type.UInt32)) return AzothValue.U32(left.U32Value * right.U32Value);
        if (type.Equals(Type.Int64)) return AzothValue.I64(left.I64Value * right.I64Value);
        if (type.Equals(Type.UInt64)) return AzothValue.U64(left.U64Value * right.U64Value);
        if (type.Equals(Type.Offset)) return AzothValue.Offset(left.OffsetValue * right.OffsetValue);
        if (type.Equals(Type.Size)) return AzothValue.Size(left.SizeValue * right.SizeValue);
        if (type.Equals(Type.NInt)) return AzothValue.NInt(left.NIntValue * right.NIntValue);
        if (type.Equals(Type.NUInt)) return AzothValue.NUInt(left.NUIntValue * right.NUIntValue);
        throw new NotImplementedException($"Multiply {type.ToILString()}");
    }

    private async ValueTask<AzothValue> DivideAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariableScope variables)
    {
        if (!leftExp.Type.Equals(rightExp.Type))
            throw new InvalidOperationException(
                $"Can't divide expressions of type {leftExp.Type} and {rightExp.Type}");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = leftExp.Type;
        if (type.Equals(Type.Int)) return AzothValue.Int(left.IntValue / right.IntValue);
        if (type.Equals(Type.UInt)) return AzothValue.Int(left.IntValue / right.IntValue);
        if (type.Equals(Type.Int8)) return AzothValue.I8((sbyte)(left.I8Value / right.I8Value));
        if (type.Equals(Type.Byte)) return AzothValue.Byte((byte)(left.ByteValue / right.ByteValue));
        if (type.Equals(Type.Int16)) return AzothValue.I16((short)(left.I16Value / right.I16Value));
        if (type.Equals(Type.UInt16)) return AzothValue.U16((ushort)(left.U16Value / right.U16Value));
        if (type.Equals(Type.Int32)) return AzothValue.I32(left.I32Value / right.I32Value);
        if (type.Equals(Type.UInt32)) return AzothValue.U32(left.U32Value / right.U32Value);
        if (type.Equals(Type.Int64)) return AzothValue.I64(left.I64Value / right.I64Value);
        if (type.Equals(Type.UInt64)) return AzothValue.U64(left.U64Value / right.U64Value);
        if (type.Equals(Type.Offset)) return AzothValue.Offset(left.OffsetValue / right.OffsetValue);
        if (type.Equals(Type.Size)) return AzothValue.Size(left.SizeValue / right.SizeValue);
        if (type.Equals(Type.NInt)) return AzothValue.NInt(left.NIntValue / right.NIntValue);
        if (type.Equals(Type.NUInt)) return AzothValue.NUInt(left.NUIntValue / right.NUIntValue);
        throw new NotImplementedException($"Divide {type.ToILString()}");
    }

    private async ValueTask<bool> BuiltInEqualsAsync(PlainType commonPlainType, IExpressionNode leftExp, IExpressionNode rightExp, LocalVariableScope variables)
    {
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        if (commonPlainType is OptionalPlainType optionalType)
        {
            if (left.IsNone && right.IsNone) return true;
            if (left.IsNone || right.IsNone) return false;
            return BuiltInEqualsAsync(optionalType.Referent, left, right);
        }

        return BuiltInEqualsAsync(commonPlainType, left, right);
    }

    private static bool BuiltInEqualsAsync(PlainType type, AzothValue left, AzothValue right)
    {
        if (type.Equals(PlainType.Int)) return left.IntValue.Equals(right.IntValue);
        if (type.Equals(PlainType.UInt)) return left.IntValue.Equals(right.IntValue);
        if (type.Equals(PlainType.Int8)) return left.I8Value.Equals(right.I8Value);
        if (type.Equals(PlainType.Byte)) return left.ByteValue.Equals(right.ByteValue);
        if (type.Equals(PlainType.Int16)) return left.I16Value.Equals(right.I16Value);
        if (type.Equals(PlainType.UInt16)) return left.U16Value.Equals(right.U16Value);
        if (type.Equals(PlainType.Int32)) return left.I32Value.Equals(right.I32Value);
        if (type.Equals(PlainType.UInt32)) return left.U32Value.Equals(right.U32Value);
        if (type.Equals(PlainType.Int64)) return left.I64Value.Equals(right.I64Value);
        if (type.Equals(PlainType.UInt64)) return left.U64Value.Equals(right.U64Value);
        if (type.Equals(PlainType.Offset)) return left.OffsetValue.Equals(right.OffsetValue);
        if (type.Equals(PlainType.Size)) return left.SizeValue.Equals(right.SizeValue);
        if (type.Equals(PlainType.NInt)) return left.NIntValue.Equals(right.NIntValue);
        if (type.Equals(PlainType.NUInt)) return left.NUIntValue.Equals(right.NUIntValue);
        throw new NotImplementedException($"Compare equality of `{type}`.");
    }

    private async ValueTask<bool> ReferenceEqualsAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariableScope variables)
    {
        if (leftExp.Type is not CapabilityType { TypeConstructor.Semantics: TypeSemantics.Reference }
            || rightExp.Type is not CapabilityType { TypeConstructor.Semantics: TypeSemantics.Reference })
            throw new InvalidOperationException(
                $"Can't compare expressions of type {leftExp.Type.ToILString()} and {rightExp.Type.ToILString()} for reference equality.");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = leftExp.Type.Known();
        if (type is OptionalType)
        {
            if (left.IsNone && right.IsNone) return true;
            if (left.IsNone || right.IsNone) return false;
        }

        return ReferenceEquals(left.ObjectValue, right.ObjectValue);
    }

    private async ValueTask<int> CompareAsync(IExpressionNode leftExp, IExpressionNode rightExp, LocalVariableScope variables)
    {
        if (!leftExp.Type.Equals(rightExp.Type))
            throw new InvalidOperationException(
                $"Can't compare expressions of type {leftExp.Type.ToILString()} and {rightExp.Type.ToILString()}.");
        var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
        var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
        var type = (Type)leftExp.Type;
        if (type is OptionalType optionalType)
        {
            if (left.IsNone && right.IsNone) return 0;
            if (left.IsNone || right.IsNone) throw new NotImplementedException("No comparison order");
            return CompareAsync(optionalType.Referent, left, right);
        }
        return CompareAsync(type, left, right);
    }

    private static int CompareAsync(Type type, AzothValue left, AzothValue right)
    {
        if (type.Equals(Type.Int)) return left.IntValue.CompareTo(right.IntValue);
        if (type.Equals(Type.UInt)) return left.IntValue.CompareTo(right.IntValue);
        if (type.Equals(Type.Int8)) return left.I8Value.CompareTo(right.I8Value);
        if (type.Equals(Type.Byte)) return left.ByteValue.CompareTo(right.ByteValue);
        if (type.Equals(Type.Int16)) return left.I16Value.CompareTo(right.I16Value);
        if (type.Equals(Type.UInt16)) return left.U16Value.CompareTo(right.U16Value);
        if (type.Equals(Type.Int32)) return left.I32Value.CompareTo(right.I32Value);
        if (type.Equals(Type.UInt32)) return left.U32Value.CompareTo(right.U32Value);
        if (type.Equals(Type.Int64)) return left.I64Value.CompareTo(right.I64Value);
        if (type.Equals(Type.UInt64)) return left.U64Value.CompareTo(right.U64Value);
        if (type.Equals(Type.Offset)) return left.OffsetValue.CompareTo(right.OffsetValue);
        if (type.Equals(Type.Size)) return left.SizeValue.CompareTo(right.SizeValue);
        if (type.Equals(Type.NInt)) return left.NIntValue.CompareTo(right.NIntValue);
        if (type.Equals(Type.NUInt)) return left.NUIntValue.CompareTo(right.NUIntValue);
        throw new NotImplementedException($"Compare `{type.ToILString()}`.");
    }

    private async ValueTask<AzothValue> NegateAsync(IExpressionNode expression, LocalVariableScope variables)
    {
        var value = await ExecuteAsync(expression, variables).ConfigureAwait(false);
        var type = expression.Type;
        if (type.Equals(Type.Int)) return AzothValue.Int(-value.IntValue);
        if (type.Equals(Type.Int8)) return AzothValue.I8((sbyte)-value.I8Value);
        if (type.Equals(Type.Int16)) return AzothValue.I16((short)-value.I16Value);
        if (type.Equals(Type.Int32)) return AzothValue.I32(-value.I32Value);
        if (type.Equals(Type.Int64)) return AzothValue.I64(-value.I64Value);
        if (type.Equals(Type.Offset)) return AzothValue.Offset(-value.OffsetValue);
        if (type.Equals(Type.NInt)) return AzothValue.NInt(-value.NIntValue);
        if (type is CapabilityType { TypeConstructor: IntegerLiteralTypeConstructor }) return AzothValue.Int(-value.IntValue);
        throw new NotImplementedException($"Negate {type.ToILString()}");
    }

    private static AzothValue IdentityHash(AzothValue value)
        => AzothValue.NUInt((nuint)value.ObjectValue.GetHashCode());

    private static AzothValue Remainder(
        AzothValue dividend,
        AzothValue divisor,
        CapabilityType type)
    {
        if (type.Equals(Type.Int)) return AzothValue.Int(dividend.IntValue % divisor.IntValue);
        if (type.Equals(Type.UInt)) return AzothValue.Int(dividend.IntValue % divisor.IntValue);
        if (type.Equals(Type.Int8)) return AzothValue.I8((sbyte)(dividend.I8Value % divisor.I8Value));
        if (type.Equals(Type.Byte)) return AzothValue.Byte((byte)(dividend.ByteValue % divisor.ByteValue));
        if (type.Equals(Type.Int16)) return AzothValue.I16((short)(dividend.I16Value % divisor.I16Value));
        if (type.Equals(Type.UInt16)) return AzothValue.U16((ushort)(dividend.U16Value % divisor.U16Value));
        if (type.Equals(Type.Int32)) return AzothValue.I32(dividend.I32Value % divisor.I32Value);
        if (type.Equals(Type.UInt32)) return AzothValue.U32(dividend.U32Value % divisor.U32Value);
        if (type.Equals(Type.Int64)) return AzothValue.I64(dividend.I64Value % divisor.I64Value);
        if (type.Equals(Type.UInt64)) return AzothValue.U64(dividend.U64Value % divisor.U64Value);
        if (type.Equals(Type.Offset)) return AzothValue.Offset(dividend.OffsetValue % divisor.OffsetValue);
        if (type.Equals(Type.Size)) return AzothValue.Size(dividend.SizeValue % divisor.SizeValue);
        if (type.Equals(Type.NInt)) return AzothValue.Offset(dividend.NIntValue % divisor.NIntValue);
        if (type.Equals(Type.NUInt)) return AzothValue.Size(dividend.NUIntValue % divisor.NUIntValue);
        throw new NotImplementedException($"Remainder {type.ToILString()}");
    }

    private async ValueTask<AzothValue> ToDisplayStringAsync(AzothValue value, CapabilityType type)
    {
        string displayString;
        if (type.Equals(Type.Int)) displayString = value.IntValue.ToString();
        else if (type.Equals(Type.UInt)) displayString = value.IntValue.ToString();
        else if (type.Equals(Type.Byte)) displayString = value.ByteValue.ToString();
        else if (type.Equals(Type.Int32)) displayString = value.I32Value.ToString();
        else if (type.Equals(Type.UInt32)) displayString = value.U32Value.ToString();
        else if (type.Equals(Type.Offset)) displayString = value.OffsetValue.ToString();
        else if (type.Equals(Type.Size)) displayString = value.SizeValue.ToString();
        else if (type.Equals(Type.NInt)) displayString = value.NIntValue.ToString();
        else if (type.Equals(Type.NUInt)) displayString = value.NUIntValue.ToString();
        else throw new NotImplementedException($"to_display_string({type.ToILString()})");

        return await ConstructStringAsync(displayString).ConfigureAwait(false);
    }

    private async ValueTask<AzothValue> ExecuteBlockOrResultAsync(
        IBlockOrResultNode statement,
        LocalVariableScope variables)
        => statement switch
        {
            IBlockExpressionNode b => await ExecuteAsync(b, variables).ConfigureAwait(false),
            IResultStatementNode s => await ExecuteAsync(s, variables).ConfigureAwait(false),
            _ => throw ExhaustiveMatch.Failed(statement)
        };

    private async ValueTask<AzothValue> ExecuteElseAsync(
        IElseClauseNode elseClause,
        LocalVariableScope variables)
    {
        return elseClause switch
        {
            IBlockOrResultNode exp => await ExecuteBlockOrResultAsync(exp, variables).ConfigureAwait(false),
            IIfExpressionNode exp => await ExecuteAsync(exp, variables).ConfigureAwait(false),
            _ => throw ExhaustiveMatch.Failed(elseClause)
        };
    }

    private async ValueTask ExecuteAssignmentAsync(
        IExpressionNode expression,
        AzothValue value,
        LocalVariableScope variables)
    {
        switch (expression)
        {
            default:
                throw new NotImplementedException($"Can't interpret assignment into {expression.GetType().Name}");
            case IVariableNameExpressionNode exp:
                variables[exp.ReferencedDefinition] = value;
                break;
            case IFieldAccessExpressionNode exp:
                var obj = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                // TODO handle the access operator
                obj.ObjectValue[exp.ReferencedDeclaration!.Name] = value;
                break;
        }
    }

    public Task WaitForExitAsync() => executionTask;

    public TextReader StandardOutput { get; }
    public TextReader StandardError => TextReader.Null;

    public byte ExitCode => exitCode ?? throw new InvalidOperationException("Process has not exited");
}
