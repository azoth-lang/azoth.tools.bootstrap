using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class WhileExpressionNode : ExpressionNode, IWhileExpressionNode
{
    public override IWhileExpressionSyntax Syntax { get; }
    private Child<IUntypedExpressionNode> condition;
    public IUntypedExpressionNode Condition => condition.Value;
    public IBlockExpressionNode Block { get; }

    public WhileExpressionNode(
        IWhileExpressionSyntax syntax,
        IUntypedExpressionNode condition,
        IBlockExpressionNode block)
    {
        Syntax = syntax;
        this.condition = Child.Create(this, condition);
        Block = Child.Attach(this, block);
    }
}
