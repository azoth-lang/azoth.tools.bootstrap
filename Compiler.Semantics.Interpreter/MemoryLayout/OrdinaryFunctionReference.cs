using System.Collections.Generic;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class OrdinaryFunctionReference : FunctionReference
{
    private readonly InterpreterProcess interpreterProcess;
    private readonly FunctionSymbol functionSymbol;

    public OrdinaryFunctionReference(InterpreterProcess interpreterProcess, FunctionSymbol functionSymbol)
    {
        this.interpreterProcess = interpreterProcess;
        this.functionSymbol = functionSymbol;
    }

    public override ValueTask<AzothValue> CallAsync(IReadOnlyList<AzothValue> arguments)
        => interpreterProcess.CallFunctionAsync(functionSymbol, arguments);
}
