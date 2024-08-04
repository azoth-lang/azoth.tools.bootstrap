using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

internal sealed class ControlFlowAspect
{
    public static FixedDictionary<IControlFlowNode, ControlFlowKind> Expression_ControlFlowNext(IExpressionNode node)
        => node.ControlFlowFollowing();

    public static FixedDictionary<IControlFlowNode, ControlFlowKind> Statement_ControlFlowNext(IStatementNode node)
        => node.ControlFlowFollowing();
}
