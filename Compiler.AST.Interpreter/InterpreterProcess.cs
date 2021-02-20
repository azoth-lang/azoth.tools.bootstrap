using System;
using System.IO;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter
{
    public class InterpreterProcess
    {
        private readonly Package package;
        private readonly Task executionTask;
        private int? exitCode;

        public InterpreterProcess(Package package)
        {
            if (package.EntryPoint is null) throw new ArgumentException("Package must have an entry point");
            this.package = package;
            executionTask = Task.Run(CallEntryPointAsync);
        }

        private async Task CallEntryPointAsync()
        {
            //  TODO Construct entry point arguments

            var entryPoint = package.EntryPoint!;
            var returnValue = await CallFunctionAsync(entryPoint).ConfigureAwait(false);
            var returnType = entryPoint.Symbol.ReturnDataType;
            if (returnType == DataType.Void)
                exitCode = 0;
            else if (returnType == DataType.Byte)
                exitCode = returnValue.Byte;
            else if (returnType == DataType.Int32)
                exitCode = returnValue.I32;
            else
                throw new InvalidOperationException($"Main function cannot have return type {returnType}");
        }

        private static async Task<AzothValue> CallFunctionAsync(IFunctionDeclaration function)
        {
            try
            {
                await ExecuteAsync(function.Body).ConfigureAwait(false);
                return default;
            }
            catch (Return @return)
            {
                return @return.Value;
            }
        }

        private static async ValueTask ExecuteAsync(IBodyOrBlock block)
        {
            foreach (var statement in block.Statements) await ExecuteAsync(statement).ConfigureAwait(false);
        }

        private static async ValueTask ExecuteAsync(IStatement statement)
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

        private static async ValueTask<AzothValue> ExecuteAsync(IExpression expression)
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
                    return new AzothValue(exp.Value);
            }
        }

        public Task WaitForExitAsync()
        {
            return executionTask;
        }

        public TextReader StandardOutput => TextReader.Null;
        public TextReader StandardError => TextReader.Null;

        public int ExitCode => exitCode ?? throw new InvalidOperationException("Process has not exited");
    }
}
