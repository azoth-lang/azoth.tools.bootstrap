using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class StatementNode : CodeNode, IStatementNode
{
    public abstract override IStatementSyntax Syntax { get; }
    public abstract IMaybeAntetype? ResultAntetype { get; }
    public abstract DataType? ResultType { get; }
    public abstract ValueId? ResultValueId { get; }
    public abstract IFlowState FlowStateAfter { get; }
    public abstract FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowNext { get; }
    private FixedDictionary<IControlFlowNode, ControlFlowKind>? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Inherited(ref controlFlowPreviousCached, ref controlFlowPrevious,
                ctx => CollectControlFlowPrevious(this, ctx));

    private protected StatementNode() { }

    public abstract LexicalScope GetLexicalScope();

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetLexicalScope();

    public IPreviousValueId PreviousValueId()
        => PreviousValueId(GrammarAttribute.CurrentInheritanceContext());

    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowFollowing()
        => InheritedControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());

    protected override void CollectControlFlowPrevious(
        IControlFlowNode target,
        Dictionary<IControlFlowNode, ControlFlowKind> previous)
    {
        ControlFlowAspect.ControlFlow_ContributeControlFlowPrevious(this, target, previous);
        base.CollectControlFlowPrevious(target, previous);
    }
}
