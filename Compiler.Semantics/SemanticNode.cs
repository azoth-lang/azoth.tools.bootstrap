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
        IControlFlowNode target,
        Dictionary<IControlFlowNode, ControlFlowKind> previous)
    {
        foreach (var child in Children().Cast<SemanticNode>())
            child.CollectControlFlowPrevious(target, previous);
    }

    protected ControlFlowSet CollectControlFlowPrevious(IControlFlowNode target, IInheritanceContext ctx)
    {
        if (this is IExecutableDefinitionNode) return CollectControlFlowPrevious(target);
        return GetParent(ctx)?.CollectControlFlowPrevious(target, ctx)
            ?? throw Child.ParentMissing(target);
    }
}
