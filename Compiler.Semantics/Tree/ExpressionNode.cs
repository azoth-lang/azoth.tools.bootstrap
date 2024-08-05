using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ExpressionNode : AmbiguousExpressionNode, IExpressionNode
{
    protected sealed override bool MayHaveRewrite => true;

    public abstract override ITypedExpressionSyntax Syntax { get; }
    private ValueId valueId;
    private bool valueIdCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref SyncLock,
                ExpressionTypesAspect.Expression_ValueId);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype, InheritedExpectedAntetype);
    // TODO make this abstract once all expressions have type implemented
    public virtual IMaybeExpressionAntetype Antetype
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(Antetype)} not implemented.");
    // TODO make this abstract once all expressions have type implemented
    public virtual DataType Type
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(Type)} not implemented.");
    // TODO make this abstract once all expressions have flow state implemented
    public virtual IFlowState FlowStateAfter
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(FlowStateAfter)} not implemented.");
    private FixedDictionary<IControlFlowNode, ControlFlowKind>? controlFlowNext;
    private bool controlFlowNextCached;
    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private FixedDictionary<IControlFlowNode, ControlFlowKind>? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Inherited(ref controlFlowPreviousCached, ref controlFlowPrevious,
                ctx => CollectControlFlowPrevious(this, ctx));

    private protected ExpressionNode() { }

    public IPreviousValueId PreviousValueId()
        => PreviousValueId(GrammarAttribute.CurrentInheritanceContext());

    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowFollowing()
        => InheritedControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());

    internal override IPreviousValueId PreviousValueId(IChildNode before, IInheritanceContext ctx) => ValueId;

    // TODO remove once all nodes properly provide the expected antetype
    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => null;

    protected override void CollectControlFlowPrevious(IControlFlowNode target, Dictionary<IControlFlowNode, ControlFlowKind> previous)
    {
        ControlFlowAspect.ControlFlow_ContributeControlFlowPrevious(this, target, previous);
        base.CollectControlFlowPrevious(target, previous);
    }

    protected override IChildNode? Rewrite()
        => ExpressionAntetypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}
