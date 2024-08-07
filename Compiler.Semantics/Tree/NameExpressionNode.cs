using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class NameExpressionNode : AmbiguousNameExpressionNode, INameExpressionNode
{
    private FixedDictionary<IControlFlowNode, ControlFlowKind>? controlFlowNext;
    private bool controlFlowNextCached;
    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached)
            ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                _ => ComputeControlFlowNext());
    private FixedDictionary<IControlFlowNode, ControlFlowKind>? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Inherited(ref controlFlowPreviousCached, ref controlFlowPrevious,
                ctx => CollectControlFlowPrevious(this, ctx));

    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowFollowing()
        => InheritedControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());

    protected override void CollectControlFlowPrevious(
        IControlFlowNode target,
        Dictionary<IControlFlowNode, ControlFlowKind> previous)
    {
        ControlFlowAspect.ControlFlow_ContributeControlFlowPrevious(this, target, previous);
        base.CollectControlFlowPrevious(target, previous);
    }

    protected virtual FixedDictionary<IControlFlowNode, ControlFlowKind> ComputeControlFlowNext()
        => ControlFlowAspect.Expression_ControlFlowNext(this);
}
