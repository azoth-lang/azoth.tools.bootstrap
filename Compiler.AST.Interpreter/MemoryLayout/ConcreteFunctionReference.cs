using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;

internal class ConcreteFunctionReference : FunctionReference
{
    private readonly InterpreterProcess interpreterProcess;
    private readonly IConcreteFunctionInvocableDeclaration function;

    public ConcreteFunctionReference(InterpreterProcess interpreterProcess, IConcreteFunctionInvocableDeclaration function)
    {
        this.interpreterProcess = interpreterProcess;
        this.function = function;
    }

    public override Task<AzothValue> CallAsync(IEnumerable<AzothValue> arguments)
        => interpreterProcess.CallFunctionAsync(function, arguments);
}
