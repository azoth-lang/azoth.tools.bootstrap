using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal class ConcreteFunctionReference : FunctionReference
{
    private readonly InterpreterProcess interpreterProcess;
    private readonly IConcreteFunctionInvocableDefinitionNode function;

    public ConcreteFunctionReference(InterpreterProcess interpreterProcess, IConcreteFunctionInvocableDefinitionNode function)
    {
        this.interpreterProcess = interpreterProcess;
        this.function = function;
    }

    public override Task<AzothValue> CallAsync(IEnumerable<AzothValue> arguments)
        => interpreterProcess.CallFunctionAsync(function, arguments);
}
