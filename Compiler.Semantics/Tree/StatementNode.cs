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
    private FixedDictionary<IControlFlowNode, ControlFlowKind>? controlFlowNext;
    private bool controlFlowNextCached;
    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Statement_ControlFlowNext);

    private protected StatementNode() { }

    public abstract LexicalScope GetLexicalScope();

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetLexicalScope();

    public IPreviousValueId PreviousValueId()
        => PreviousValueId(GrammarAttribute.CurrentInheritanceContext());

    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowFollowing()
        => InheritedControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
}
