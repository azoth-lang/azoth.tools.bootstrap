using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ExpressionNode : AmbiguousExpressionNode, IExpressionNode
{
    protected sealed override bool MayHaveRewrite => true;

    public abstract override IExpressionSyntax Syntax { get; }
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype, Inherited_ExpectedAntetype);
    // TODO make this abstract once all expressions have type implemented
    public virtual IMaybeExpressionAntetype Antetype
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(Antetype)} not implemented.");
    private DataType? expectedType;
    private bool expectedTypeCached;
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType, Inherited_ExpectedType);
    // TODO make this abstract once all expressions have type implemented
    public virtual DataType Type
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(Type)} not implemented.");
    // TODO make this abstract once all expressions have flow state implemented
    public virtual IFlowState FlowStateAfter
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(FlowStateAfter)} not implemented.");
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                _ => ComputeControlFlowNext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    private protected ExpressionNode() { }

    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());

    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());

    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());

    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx) => ValueId;

    // TODO remove once all nodes properly provide the expected antetype
    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => null;

    // TODO remove once all nodes properly provide the expected type
    internal override DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => null;

    protected virtual ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.Expression_ControlFlowNext(this);

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // By default, implicit recovery is not allowed
        => false;

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => false;

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionAntetypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? base.Rewrite();
}
