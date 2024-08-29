using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

/// <summary>
/// A temporary partial class to make the transition to generated tree nodes possible.
/// </summary>
internal partial class SemanticNode
{
    public virtual IEnumerable<ISemanticNode> Children() => ((ISemanticNode)this).Children();

    private ControlFlowSet CollectControlFlowPrevious(IControlFlowNode target)
    {
        var previous = new Dictionary<IControlFlowNode, ControlFlowKind>();
        CollectControlFlowPrevious(target, previous);
        return ControlFlowSet.Create(previous);
    }

    protected virtual void CollectControlFlowPrevious(
        IControlFlowNode before,
        Dictionary<IControlFlowNode, ControlFlowKind> previous)
    {
        foreach (var child in Children().Cast<SemanticNode>())
            child.CollectControlFlowPrevious(before, previous);
    }

    internal ControlFlowSet CollectControlFlowPrevious(IControlFlowNode before, IInheritanceContext ctx)
    {
        if (this is IExecutableDefinitionNode) return CollectControlFlowPrevious(before);
        return GetParent(ctx)?.CollectControlFlowPrevious(before, ctx)
            ?? throw Child.PreviousFailed("ControlFlowPrevious", before);
    }
}
