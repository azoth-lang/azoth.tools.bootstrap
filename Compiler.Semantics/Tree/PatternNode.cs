using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class PatternNode : CodeNode, IPatternNode
{
    public abstract override IPatternSyntax Syntax { get; }
    public virtual ValueId? ValueId => null;
    public FlowState FlowStateAfter => throw new System.NotImplementedException();

    private protected PatternNode() { }

    public virtual LexicalScope GetContainingLexicalScope() => InheritedContainingLexicalScope();

    public abstract ConditionalLexicalScope GetFlowLexicalScope();

    public IFlowNode Predecessor() => throw new System.NotImplementedException();

    public FlowState FlowStateBefore() => throw new System.NotImplementedException();

    public new IPreviousValueId PreviousValueId() => base.PreviousValueId();
}
