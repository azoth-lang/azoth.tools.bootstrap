using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AssignmentExpressionNode : ExpressionNode, IAssignmentExpressionNode
{
    public override IAssignmentExpressionSyntax Syntax { get; }
    private Child<IAssignableExpressionNode> leftOperand;
    public IAssignableExpressionNode LeftOperand => leftOperand.Value;
    public AssignmentOperator Operator => Syntax.Operator;
    private Child<IUntypedExpressionNode> rightOperand;
    public IUntypedExpressionNode RightOperand => rightOperand.Value;

    public AssignmentExpressionNode(
        IAssignmentExpressionSyntax syntax,
        IAssignableExpressionNode leftOperand,
        IUntypedExpressionNode rightOperand)
    {
        Syntax = syntax;
        this.leftOperand = Child.Create(this, leftOperand);
        this.rightOperand = Child.Create(this, rightOperand);
    }
}