using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ExitNode : ControlFlowNode, IExitNode
{
    public override FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowNext
        => FixedDictionary<IControlFlowNode, ControlFlowKind>.Empty;
}
