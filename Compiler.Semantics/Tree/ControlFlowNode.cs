using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ControlFlowNode : ChildNode, IControlFlowNode
{
    public sealed override ICodeSyntax? Syntax => null;
    public CodeFile File => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public abstract ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public abstract IEntryNode ControlFlowEntry();

    public virtual ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());

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
