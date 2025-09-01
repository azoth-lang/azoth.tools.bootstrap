using System.Collections.Generic;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class MethodReference : FunctionReference
{
    private readonly InterpreterProcess interpreterProcess;
    private readonly Type selfType;
    private readonly Value self;
    private readonly MethodSymbol methodSymbol;

    public override FunctionType FunctionType => methodSymbol.MethodReferenceType;

    public MethodReference(
        InterpreterProcess interpreterProcess,
        Type selfType,
        Value self,
        MethodSymbol methodSymbol)
    {
        this.interpreterProcess = interpreterProcess;
        this.self = self;
        this.methodSymbol = methodSymbol;
        this.selfType = selfType;
    }

    public override ValueTask<Value> CallAsync(IReadOnlyList<Value> arguments)
        => interpreterProcess.CallMethodAsync(methodSymbol, selfType, self, arguments);
}
