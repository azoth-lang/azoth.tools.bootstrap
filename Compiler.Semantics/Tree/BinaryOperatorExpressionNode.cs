using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BinaryOperatorExpressionNode : ExpressionNode, IBinaryOperatorExpressionNode
{
    public override IBinaryOperatorExpressionSyntax Syntax { get; }
    private Child<IUntypedExpressionNode> leftOperand;
    public IUntypedExpressionNode LeftOperand => leftOperand.Value;
    public BinaryOperator Operator => Syntax.Operator;
    private Child<IUntypedExpressionNode> rightOperand;
    public IUntypedExpressionNode RightOperand => rightOperand.Value;

    public BinaryOperatorExpressionNode(
        IBinaryOperatorExpressionSyntax syntax,
        IUntypedExpressionNode leftOperand,
        IUntypedExpressionNode rightOperand)
    {
        Syntax = syntax;
        this.leftOperand = Child.Create(this, leftOperand);
        this.rightOperand = Child.Create(this, rightOperand);
    }
}
