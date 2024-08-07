using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

internal static class ControlFlowSet
{
    public static FixedDictionary<IControlFlowNode, ControlFlowKind> Empty => FixedDictionary<IControlFlowNode, ControlFlowKind>.Empty;

    public static FixedDictionary<IControlFlowNode, ControlFlowKind> CreateNormal(params IControlFlowNode[] nodes)
        => nodes.ToFixedDictionary(Functions.Identity, _ => ControlFlowKind.Normal);

    public static FixedDictionary<IControlFlowNode, ControlFlowKind> CreateNormal(IControlFlowNode? node)
        => node is null ? Empty : new([KeyValuePair.Create(node, ControlFlowKind.Normal)]);

    public static FixedDictionary<IControlFlowNode, ControlFlowKind> CreateNormal(IControlFlowNode node1, IControlFlowNode node2)
        => new([KeyValuePair.Create(node1, ControlFlowKind.Normal), KeyValuePair.Create(node2, ControlFlowKind.Normal)]);
}
