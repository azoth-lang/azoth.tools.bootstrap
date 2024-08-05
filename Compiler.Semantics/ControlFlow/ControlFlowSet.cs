using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

internal static class ControlFlowSet
{
    public static FixedDictionary<IControlFlowNode, ControlFlowKind> CreateNormal(params IControlFlowNode[] nodes)
        => nodes.ToFixedDictionary(Functions.Identity, _ => ControlFlowKind.Normal);
}
