using System.Collections.Generic;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class InitializerReference : FunctionReference
{
    private readonly InterpreterProcess interpreterProcess;
    private readonly BareType bareType;
    private readonly InitializerSymbol initializerSymbol;

    public override FunctionType FunctionType => initializerSymbol.InitializerGroupType;

    public InitializerReference(InterpreterProcess interpreterProcess, BareType bareType, InitializerSymbol initializerSymbol)
    {
        this.interpreterProcess = interpreterProcess;
        this.bareType = bareType;
        this.initializerSymbol = initializerSymbol;
    }

    public override ValueTask<AzothValue> CallAsync(IReadOnlyList<AzothValue> arguments)
        => interpreterProcess.CallInitializerAsync(bareType, initializerSymbol, arguments);
}
