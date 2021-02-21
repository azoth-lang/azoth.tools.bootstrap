using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter
{
    public class InterpreterProcess
    {
        private readonly Package package;
        private readonly Task executionTask;
        private readonly FixedDictionary<FunctionSymbol, IConcreteFunctionInvocableDeclaration> functions;
        private readonly FixedDictionary<ConstructorSymbol, IConstructorDeclaration?> constructors;
        private readonly FixedDictionary<ObjectTypeSymbol, IClassDeclaration> classes;
        private readonly IClassDeclaration stringClass;
        private readonly IConstructorDeclaration stringConstructor;
        private byte? exitCode;
        private readonly MemoryStream standardOutput;
        private readonly TextWriter standardOutputWriter;
        private readonly MethodSignatureCache methodSignatures = new MethodSignatureCache();
        private readonly ConcurrentDictionary<IClassDeclaration, VTable> vTables = new ConcurrentDictionary<IClassDeclaration, VTable>();

        public InterpreterProcess(Package package)
        {
            if (package.EntryPoint is null) throw new ArgumentException("Package must have an entry point");
            this.package = package;
            var allDeclarations = package.AllDeclarations.Concat(package.References.SelectMany(r => r.AllDeclarations))
                                         .ToList();
            functions = allDeclarations
                        .OfType<IConcreteFunctionInvocableDeclaration>()
                        .ToFixedDictionary(f => f.Symbol);
            var defaultConstructorSymbols = allDeclarations
                                            .OfType<IClassDeclaration>()
                                            .Select(c => c.DefaultConstructorSymbol)
                                            .WhereNotNull();
            classes = allDeclarations.OfType<IClassDeclaration>()
                                     .ToFixedDictionary(c => c.Symbol);
            stringClass = classes.Values.Single(c => c.Symbol.Name == "string");
            stringConstructor = stringClass.Members.OfType<IConstructorDeclaration>().Single();
            constructors = defaultConstructorSymbols
                           .Select(c => (c, default(IConstructorDeclaration)))
                           .Concat(package.AllDeclarations
                                      .OfType<IConstructorDeclaration>()
                                      .Select(c => (c.Symbol, (IConstructorDeclaration?)c)))
                           .ToFixedDictionary();
            executionTask = Task.Run(CallEntryPointAsync);

            standardOutput = new MemoryStream();
            standardOutputWriter = new StreamWriter(standardOutput, Encoding.UTF8, leaveOpen: true);
            StandardOutput = new StreamReader(standardOutput, Encoding.UTF8);
        }

        private async Task CallEntryPointAsync()
        {
            try
            {
                var entryPoint = package.EntryPoint!;
                var arguments = new List<AzothValue>();
                foreach (var parameterType in entryPoint.Symbol.ParameterDataTypes)
                    if (parameterType is ObjectType objectType && objectType.Name.Text == "TestOutput")
                    {
                        var testOutputDeclaration = classes.Values.SingleOrDefault(c => c.Symbol.Name == "TestOutput");
                        if (testOutputDeclaration is null)
                            throw new InvalidOperationException("No TestOutput type declared");
                        var @class = classes[testOutputDeclaration.Symbol];
                        var vTable = vTables.GetOrAdd(@class, CreateVTable);
                        var testOutput = AzothValue.Object(new AzothObject(vTable));
                        arguments.Add(testOutput);
                    }
                    else
                        throw new InvalidOperationException($"Parameter to main of type {parameterType} not supported");

                var returnValue = await CallFunctionAsync(entryPoint, arguments).ConfigureAwait(false);
                // Flush any buffered output
                await standardOutputWriter.FlushAsync().ConfigureAwait(false);
                var returnType = entryPoint.Symbol.ReturnDataType;
                if (returnType == DataType.Void)
                    exitCode = 0;
                else if (returnType == DataType.Byte)
                    exitCode = returnValue.ByteValue;
                else
                    throw new InvalidOperationException($"Main function cannot have return type {returnType}");
            }
            finally
            {
                await standardOutputWriter.DisposeAsync().ConfigureAwait(false);
                standardOutput.Position = 0;
            }
        }

        private async Task<AzothValue> CallFunctionAsync(IConcreteFunctionInvocableDeclaration function, IEnumerable<AzothValue> arguments)
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

        private async ValueTask<AzothValue> CallConstructorAsync(
            IConstructorDeclaration constructor,
            AzothValue self,
            IEnumerable<AzothValue> arguments)
        {
            try
            {
                var variables = new LocalVariableScope();
                variables.Add(constructor.ImplicitSelfParameter.Symbol, self);
                foreach (var (arg, parameter) in arguments.Zip(constructor.Parameters))
                    switch (parameter)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(parameter);
                        case IFieldParameter fieldParameter:
                            // TODO initialize field
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

        private async ValueTask<AzothValue> CallMethodAsync(
            IMethodDeclaration method,
            AzothValue self,
            IEnumerable<AzothValue> arguments)
        {
            if (!(method is IConcreteMethodDeclaration concreteMethod))
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

        private async ValueTask ExecuteAsync(IStatement statement, LocalVariableScope variables)
        {
            switch (statement)
            {
                default:
                    throw new NotImplementedException($"Can't interpret {statement.GetType().Name}");
                case IExpressionStatement s:
                    await ExecuteAsync(s.Expression, variables).ConfigureAwait(false);
                    break;
                case IVariableDeclarationStatement d:
                {
                    var initialValue = d.Initializer is null
                        ? AzothValue.None
                        : await ExecuteAsync(d.Initializer, variables).ConfigureAwait(false);
                    variables.Add(d.Symbol, initialValue);
                }
                break;
            }
        }

        private async ValueTask<AzothValue> ExecuteAsync(IExpression expression, LocalVariableScope variables)
        {
            switch (expression)
            {
                default:
                    throw new NotImplementedException($"Can't interpret {expression.GetType().Name}");
                case IReturnExpression exp:
                    if (exp.Value is null) throw new Return();
                    throw new Return(await ExecuteAsync(exp.Value, variables).ConfigureAwait(false));
                case IImplicitNumericConversionExpression exp:
                {
                    var value = await ExecuteAsync(exp.Expression, variables).ConfigureAwait(false);
                    return value.Convert(exp.Expression.DataType, exp.ConvertToType);
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
                case IBoolLiteralExpression exp:
                    return AzothValue.Bool(exp.Value);
                case IIfExpression exp:
                {
                    var condition = await ExecuteAsync(exp.Condition, variables).ConfigureAwait(false);
                    if (condition.BoolValue)
                        return await ExecuteBlockOrResultAsync(exp.ThenBlock, variables).ConfigureAwait(false);
                    if (exp.ElseClause != null)
                        return await ExecuteElseAsync(exp.ElseClause, variables).ConfigureAwait(false);
                    return AzothValue.None;
                }
                case INameExpression exp:
                    return variables[exp.ReferencedSymbol];
                case IBlockExpression block:
                {
                    var blockVariables = new LocalVariableScope(variables);
                    foreach (var statement in block.Statements)
                    {
                        if (statement is IResultStatement resultStatement)
                            return await ExecuteAsync(resultStatement.Expression, blockVariables).ConfigureAwait(false);
                        await ExecuteAsync(statement, blockVariables).ConfigureAwait(false);
                    }
                    return AzothValue.None;
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
                case INextExpression:
                    throw new Next();
                case IAssignmentExpression exp:
                {
                    var value = await ExecuteAsync(exp.RightOperand, variables).ConfigureAwait(false);
                    // TODO the expression being assigned into is supposed to be evaluated first
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
                            return AzothValue.Bool(await CompareAsync(exp.LeftOperand, exp.RightOperand, variables)
                                .ConfigureAwait(false) == 0);
                        case BinaryOperator.NotEqual:
                            return AzothValue.Bool(await CompareAsync(exp.LeftOperand, exp.RightOperand, variables)
                                .ConfigureAwait(false) != 0);
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
                            throw new NotImplementedException($"Operator {exp.Operator}");
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
                    var methodSignature = methodSignatures[exp.ReferencedSymbol];

                    var selfType = exp.Context.DataType;
                    switch (selfType)
                    {
                        case EmptyType _:
                        case UnknownType _:
                        case BoolType _:
                        case OptionalType _:
                            throw new InvalidOperationException($"Can't call {methodSignature} on {selfType}");
                        case ReferenceType _:
                            var vtable = self.ObjectValue.VTable;
                            var method = vtable[methodSignature];
                            return await CallMethodAsync(method, self, arguments).ConfigureAwait(false);
                        case NumericType numericType:
                            if (methodSignature.Name.Text == "remainder")
                                return Remainder(self, arguments[0], numericType);

                            throw new InvalidOperationException($"Can't call {methodSignature} on {selfType}");
                        default:
                            throw ExhaustiveMatch.Failed(selfType);
                    }
                }
                case INewObjectExpression exp:
                {
                    var arguments = await ExecuteArgumentsAsync(exp.Arguments, variables).ConfigureAwait(false);
                    var objectTypeSymbol = exp.ReferencedSymbol.ContainingSymbol;
                    var @class = classes[objectTypeSymbol];
                    var vTable = vTables.GetOrAdd(@class, CreateVTable);
                    var self = AzothValue.Object(new AzothObject(vTable));
                    var constructor = constructors[exp.ReferencedSymbol];
                    // Default constructor is null
                    if (constructor is null) return self;
                    return await CallConstructorAsync(constructor, self, arguments).ConfigureAwait(false);
                }
                case IShareExpression exp:
                    // TODO do share expressions make sense in Azoth?
                    return await ExecuteAsync(exp.Referent, variables).ConfigureAwait(false);
                case IBorrowExpression exp:
                    // TODO do borrow expressions make sense in Azoth?
                    return await ExecuteAsync(exp.Referent, variables).ConfigureAwait(false);
                case ISelfExpression exp:
                    return variables[exp.ReferencedSymbol];
                case IImplicitNoneConversionExpression exp:
                    return AzothValue.None;
                case IImplicitOptionalConversionExpression exp:
                    return await ExecuteAsync(exp.Expression, variables).ConfigureAwait(false);
                case IStringLiteralExpression exp:
                {
                    // Call the constructor of the string class
                    var arguments = new List<AzothValue>
                    {
                        AzothValue.Size((nuint) exp.Value.Length),
                        AzothValue.Bytes(Encoding.UTF8.GetBytes(exp.Value))
                    };
                    var @class = stringClass;
                    var vTable = vTables.GetOrAdd(@class, CreateVTable);
                    var self = AzothValue.Object(new AzothObject(vTable));
                    return await CallConstructorAsync(stringConstructor, self, arguments).ConfigureAwait(false);
                }
                case IUnsafeExpression exp:
                    return await ExecuteAsync(exp.Expression, variables).ConfigureAwait(false);
                case IFieldAccessExpression exp:
                {
                    var obj = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                    return obj.ObjectValue[exp.ReferencedSymbol.Name];
                }
            }
        }

        private async ValueTask<AzothValue> CallIntrinsicAsync(FunctionSymbol functionSymbol, List<AzothValue> arguments)
        {
            if (functionSymbol == Intrinsic.PrintUtf8)
            {
                var str = Encoding.UTF8.GetString(arguments[0].BytesValue, 0, (int)arguments[1].SizeValue);
                await standardOutputWriter.WriteAsync(str).ConfigureAwait(false);
                return AzothValue.None;
            }
            throw new NotImplementedException($"Intrinsic {functionSymbol}");
        }

        private VTable CreateVTable(IClassDeclaration @class)
        {
            return new VTable(@class, methodSignatures);
        }

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
                    $"Can't add expressions of type {leftExp.DataType} and {rightExp.DataType}");
            var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
            var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
            var dataType = leftExp.DataType;
            if (dataType == DataType.Byte)
                return AzothValue.Byte((byte)(left.ByteValue + right.ByteValue));
            if (dataType == DataType.Int32)
                return AzothValue.I32(left.I32Value + right.I32Value);
            if (dataType == DataType.UInt32)
                return AzothValue.U32(left.U32Value + right.U32Value);
            if (dataType == DataType.Offset)
                return AzothValue.Offset(left.OffsetValue + right.OffsetValue);
            if (dataType == DataType.Size)
                return AzothValue.Size(left.SizeValue * right.SizeValue);
            throw new NotImplementedException($"Add {dataType}");
        }

        private async ValueTask<AzothValue> SubtractAsync(IExpression leftExp, IExpression rightExp, LocalVariableScope variables)
        {
            if (leftExp.DataType != rightExp.DataType)
                throw new InvalidOperationException(
                    $"Can't subtract expressions of type {leftExp.DataType} and {rightExp.DataType}");
            var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
            var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
            var dataType = leftExp.DataType;
            if (dataType == DataType.Byte)
                return AzothValue.Byte((byte)(left.ByteValue - right.ByteValue));
            if (dataType == DataType.Int32)
                return AzothValue.I32(left.I32Value - right.I32Value);
            if (dataType == DataType.UInt32)
                return AzothValue.U32(left.U32Value - right.U32Value);
            if (dataType == DataType.Offset)
                return AzothValue.Offset(left.OffsetValue - right.OffsetValue);
            if (dataType == DataType.Size)
                return AzothValue.Size(left.SizeValue - right.SizeValue);
            throw new NotImplementedException($"Subtract {dataType}");
        }

        private async ValueTask<AzothValue> MultiplyAsync(IExpression leftExp, IExpression rightExp, LocalVariableScope variables)
        {
            if (leftExp.DataType != rightExp.DataType)
                throw new InvalidOperationException(
                    $"Can't multiply expressions of type {leftExp.DataType} and {rightExp.DataType}");
            var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
            var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
            var dataType = leftExp.DataType;
            if (dataType == DataType.Byte)
                return AzothValue.Byte((byte)(left.ByteValue * right.ByteValue));
            if (dataType == DataType.Int32)
                return AzothValue.I32(left.I32Value * right.I32Value);
            if (dataType == DataType.UInt32)
                return AzothValue.U32(left.U32Value * right.U32Value);
            if (dataType == DataType.Offset)
                return AzothValue.Offset(left.OffsetValue * right.OffsetValue);
            if (dataType == DataType.Size)
                return AzothValue.Size(left.SizeValue * right.SizeValue);
            throw new NotImplementedException($"Multiply {dataType}");
        }

        private async ValueTask<AzothValue> DivideAsync(IExpression leftExp, IExpression rightExp, LocalVariableScope variables)
        {
            if (leftExp.DataType != rightExp.DataType)
                throw new InvalidOperationException(
                    $"Can't divide expressions of type {leftExp.DataType} and {rightExp.DataType}");
            var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
            var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
            var dataType = leftExp.DataType;
            if (dataType == DataType.Byte)
                return AzothValue.Byte((byte)(left.ByteValue / right.ByteValue));
            if (dataType == DataType.Int32)
                return AzothValue.I32(left.I32Value / right.I32Value);
            if (dataType == DataType.UInt32)
                return AzothValue.U32(left.U32Value / right.U32Value);
            if (dataType == DataType.Offset)
                return AzothValue.Offset(left.OffsetValue / right.OffsetValue);
            if (dataType == DataType.Size)
                return AzothValue.Size(left.SizeValue / right.SizeValue);
            throw new NotImplementedException($"Divide {dataType}");
        }

        private async ValueTask<int> CompareAsync(IExpression leftExp, IExpression rightExp, LocalVariableScope variables)
        {
            if (leftExp.DataType != rightExp.DataType)
                throw new InvalidOperationException(
                    $"Can't compare expressions of type {leftExp.DataType} and {rightExp.DataType}");
            var left = await ExecuteAsync(leftExp, variables).ConfigureAwait(false);
            var right = await ExecuteAsync(rightExp, variables).ConfigureAwait(false);
            var dataType = leftExp.DataType;
            if (dataType == DataType.Byte)
                return left.ByteValue.CompareTo(right.ByteValue);
            if (dataType == DataType.Int32)
                return left.I32Value.CompareTo(right.I32Value);
            if (dataType == DataType.UInt32)
                return left.U32Value.CompareTo(right.U32Value);
            if (dataType == DataType.Offset)
                return left.OffsetValue.CompareTo(right.OffsetValue);
            if (dataType == DataType.Size)
                return left.SizeValue.CompareTo(right.SizeValue);
            throw new NotImplementedException($"Compare {dataType}");
        }

        private async ValueTask<AzothValue> NegateAsync(IExpression expression, LocalVariableScope variables)
        {
            var value = await ExecuteAsync(expression, variables).ConfigureAwait(false);
            var dataType = expression.DataType;
            if (dataType == DataType.Int32)
                return AzothValue.I32(-value.I32Value);
            if (dataType is IntegerConstantType)
                return AzothValue.Int(-value.IntValue);
            throw new NotImplementedException($"Negate {dataType}");
        }

        private static AzothValue Remainder(
            AzothValue dividend,
            AzothValue divisor,
            NumericType type)
        {
            if (type == DataType.Byte)
                return AzothValue.I32(dividend.ByteValue % divisor.ByteValue);
            if (type == DataType.Int32)
                return AzothValue.I32(dividend.I32Value % divisor.I32Value);
            if (type == DataType.UInt32)
                return AzothValue.U32(dividend.U32Value % divisor.U32Value);
            if (type == DataType.Offset)
                return AzothValue.Offset(dividend.OffsetValue % divisor.OffsetValue);
            if (type == DataType.Size)
                return AzothValue.Size(dividend.SizeValue % divisor.SizeValue);
            throw new NotImplementedException($"Remainder {type}");
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
                case INameExpression exp:
                    variables[exp.ReferencedSymbol] = value;
                    break;
                case IFieldAccessExpression exp:
                    var obj = await ExecuteAsync(exp.Context, variables).ConfigureAwait(false);
                    // TODO handle the access operator
                    obj.ObjectValue[exp.ReferencedSymbol.Name] = value;
                    break;
            }
        }

        public Task WaitForExitAsync()
        {
            return executionTask;
        }

        public TextReader StandardOutput { get; }
        public TextReader StandardError => TextReader.Null;

        public byte ExitCode => exitCode ?? throw new InvalidOperationException("Process has not exited");
    }
}
