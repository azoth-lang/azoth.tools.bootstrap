using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class WhileExpressionNode : ExpressionNode, IWhileExpressionNode
{
    public override IWhileExpressionSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> condition;
    public IAmbiguousExpressionNode Condition => condition.Value;
    public IBlockExpressionNode Block { get; }

    public WhileExpressionNode(
        IWhileExpressionSyntax syntax,
        IAmbiguousExpressionNode condition,
        IBlockExpressionNode block)
    {
        Syntax = syntax;
        this.condition = Child.Create(this, condition);
        Block = Child.Attach(this, block);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == Block)
            return Condition.GetFlowLexicalScope().True;
        return base.InheritedContainingLexicalScope(child, descendant);
    }

    internal override IFlowNode InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant == Condition)
            return base.InheritedPredecessor(child, descendant);
        if (descendant == Block)
            return (IFlowNode)Condition;
        return base.InheritedPredecessor(child, descendant);
    }
}
