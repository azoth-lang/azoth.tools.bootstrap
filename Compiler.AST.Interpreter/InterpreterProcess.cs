using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter
{
    public class InterpreterProcess
    {
        private readonly Package package;
        private readonly Task executionTask;
        private readonly FixedDictionary<FunctionSymbol, IFunctionDeclaration> functions;
        private byte? exitCode;


        public InterpreterProcess(Package package)
        {
            if (package.EntryPoint is null) throw new ArgumentException("Package must have an entry point");
            this.package = package;
            functions = package.NonMemberDeclarations
                               .OfType<IFunctionDeclaration>()
                               .ToFixedDictionary(f => f.Symbol);
            executionTask = Task.Run(CallEntryPointAsync);

        }

        private async Task CallEntryPointAsync()
        {
            //  TODO Construct entry point arguments

            var entryPoint = package.EntryPoint!;
            var returnValue = await CallFunctionAsync(entryPoint, Enumerable.Empty<AzothValue>()).ConfigureAwait(false);
            var returnType = entryPoint.Symbol.ReturnDataType;
            if (returnType == DataType.Void)
                exitCode = 0;
            else if (returnType == DataType.Byte)
                exitCode = returnValue.ByteValue;
            else
                throw new InvalidOperationException($"Main function cannot have return type {returnType}");
        }

        private async Task<AzothValue> CallFunctionAsync(IFunctionDeclaration function, IEnumerable<AzothValue> arguments)
        {
            try
            {
                // TODO use the arguments
                await ExecuteAsync(function.Body).ConfigureAwait(false);
                return default;
            }
            catch (Return @return)
            {
                return @return.Value;
            }
        }

        private async ValueTask ExecuteAsync(IBodyOrBlock block)
        {
            foreach (var statement in block.Statements)
                await ExecuteAsync(statement).ConfigureAwait(false);
        }

        private async ValueTask ExecuteAsync(IStatement statement)
        {
            switch (statement)
            {
                default:
                    throw new NotImplementedException($"Can't interpret {statement.GetType().Name}");
                case IExpressionStatement s:
                    await ExecuteAsync(s.Expression).ConfigureAwait(false);
                    break;
            }
        }

        private async ValueTask<AzothValue> ExecuteAsync(IExpression expression)
        {
            switch (expression)
            {
                default:
                    throw new NotImplementedException($"Can't interpret {expression.GetType().Name}");
                case IReturnExpression exp:
                    if (exp.Value is null) throw new Return();
                    throw new Return(await ExecuteAsync(exp.Value).ConfigureAwait(false));
                case IImplicitNumericConversionExpression exp:
                {
                    var value = await ExecuteAsync(exp.Expression).ConfigureAwait(false);
                    return value.Convert(exp.Expression.DataType, exp.ConvertToType);
                }
                case IIntegerLiteralExpression exp:
                    return AzothValue.Int(exp.Value);
                case IFunctionInvocationExpression exp:
                {
                    var arguments = new List<AzothValue>(exp.Arguments.Count);
                    // Execute arguments in order
                    foreach (var argument in exp.Arguments)
                        arguments.Add(await ExecuteAsync(argument).ConfigureAwait(false));

                    return await CallFunctionAsync(functions[exp.ReferencedSymbol], arguments).ConfigureAwait(false);
                }
                case IBoolLiteralExpression exp:
                    return AzothValue.Bool(exp.Value);
            }
        }

        public Task WaitForExitAsync()
        {
            return executionTask;
        }

        public TextReader StandardOutput => TextReader.Null;
        public TextReader StandardError => TextReader.Null;

        public byte ExitCode => exitCode ?? throw new InvalidOperationException("Process has not exited");
    }
}
