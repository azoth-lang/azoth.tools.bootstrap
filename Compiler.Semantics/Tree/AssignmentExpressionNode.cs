using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AssignmentExpressionNode : ExpressionNode, IAssignmentExpressionNode
{
    public override IAssignmentExpressionSyntax Syntax { get; }
    public IAssignableExpressionNode LeftOperand { get; }
    public AssignmentOperator Operator => Syntax.Operator;
    public IUntypedExpressionNode RightOperand { get; }

    public AssignmentExpressionNode(
        IAssignmentExpressionSyntax syntax,
        IAssignableExpressionNode leftOperand,
        IUntypedExpressionNode rightOperand)
    {
        Syntax = syntax;
        LeftOperand = Child.Attach(this, leftOperand);
        RightOperand = Child.Attach(this, rightOperand);
    }
}
