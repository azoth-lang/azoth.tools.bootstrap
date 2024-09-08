using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class NameExpressionNode : AmbiguousNameExpressionNode, INameExpressionNode
{
    protected override bool MayHaveRewrite => !((IExpressionNode)this).ShouldNotBeExpression();

    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached)
            ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                _ => ComputeControlFlowNext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());

    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());

    protected virtual ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.Expression_ControlFlowNext(this);

    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());

    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());

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
