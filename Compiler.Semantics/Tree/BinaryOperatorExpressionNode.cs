using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BinaryOperatorExpressionNode : ExpressionNode, IBinaryOperatorExpressionNode
{
    public override IBinaryOperatorExpressionSyntax Syntax { get; }
    public IUntypedExpressionNode LeftOperand { get; }
    public BinaryOperator Operator => Syntax.Operator;
    public IUntypedExpressionNode RightOperand { get; }

    public BinaryOperatorExpressionNode(
        IBinaryOperatorExpressionSyntax syntax,
        IUntypedExpressionNode leftOperand,
        IUntypedExpressionNode rightOperand)
    {
        Syntax = syntax;
        LeftOperand = Child.Attach(this, leftOperand);
        RightOperand = Child.Attach(this, rightOperand);
    }
}
