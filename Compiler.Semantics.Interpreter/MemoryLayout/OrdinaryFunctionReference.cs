using System.Collections.Generic;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class OrdinaryFunctionReference : FunctionReference
{
    private readonly InterpreterProcess interpreterProcess;
    private readonly FunctionSymbol functionSymbol;

    public override FunctionType FunctionType => functionSymbol.Type;

    public OrdinaryFunctionReference(InterpreterProcess interpreterProcess, FunctionSymbol functionSymbol)
    {
        this.interpreterProcess = interpreterProcess;
        this.functionSymbol = functionSymbol;
    }

    public override ValueTask<Value> CallAsync(IReadOnlyList<Value> arguments)
        => interpreterProcess.CallFunctionAsync(functionSymbol, arguments);
}
