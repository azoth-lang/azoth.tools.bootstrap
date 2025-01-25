using System.Collections.Generic;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class InitializerReference : FunctionReference
{
    private readonly InterpreterProcess interpreterProcess;
    private readonly InitializerSymbol initializerSymbol;

    public InitializerReference(InterpreterProcess interpreterProcess, InitializerSymbol initializerSymbol)
    {
        this.interpreterProcess = interpreterProcess;
        this.initializerSymbol = initializerSymbol;
    }

    public override ValueTask<AzothValue> CallAsync(IReadOnlyList<AzothValue> arguments)
        => interpreterProcess.CallInitializerAsync(initializerSymbol, arguments);
}
