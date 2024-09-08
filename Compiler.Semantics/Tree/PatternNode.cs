using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class PatternNode : CodeNode, IPatternNode
{
    protected AttributeLock SyncLock;

    public abstract override IPatternSyntax Syntax { get; }

    public abstract IFlowState FlowStateAfter { get; }

    private ValueId? matchReferentValueId;
    private bool matchReferentValueIdCached;
    public ValueId? MatchReferentValueId
        => GrammarAttribute.IsCached(in matchReferentValueIdCached) ? matchReferentValueId
            : this.Inherited(ref matchReferentValueIdCached, ref matchReferentValueId, ref SyncLock,
                Inherited_MatchReferentValueId);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                _ => ComputeControlFlow());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;

    private protected PatternNode() { }

    public abstract ConditionalLexicalScope FlowLexicalScope();

    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());

    public IMaybeAntetype ContextBindingAntetype()
        => Inherited_ContextBindingAntetype(GrammarAttribute.CurrentInheritanceContext());

    public DataType ContextBindingType()
        => Inherited_ContextBindingType(GrammarAttribute.CurrentInheritanceContext());

    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());

    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());

    protected abstract ControlFlowSet ComputeControlFlow();

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
}
