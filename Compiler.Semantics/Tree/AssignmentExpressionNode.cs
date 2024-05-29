using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AssignmentExpressionNode : ExpressionNode, IAssignmentExpressionNode
{
    public override IAssignmentExpressionSyntax Syntax { get; }
    private Child<IAssignableExpressionNode> leftOperand;
    public IAssignableExpressionNode LeftOperand => leftOperand.Value;
    public AssignmentOperator Operator => Syntax.Operator;
    private Child<IAmbiguousExpressionNode> rightOperand;
    public IAmbiguousExpressionNode RightOperand => rightOperand.Value;

    public AssignmentExpressionNode(
        IAssignmentExpressionSyntax syntax,
        IAssignableExpressionNode leftOperand,
        IAmbiguousExpressionNode rightOperand)
    {
        Syntax = syntax;
        this.leftOperand = Child.Create(this, leftOperand);
        this.rightOperand = Child.Create(this, rightOperand);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope()
        => LexicalScopingAspect.AssignmentExpression_GetFlowLexicalScope(this);

    internal override IFlowNode InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (child == LeftOperand)
            return base.InheritedPredecessor(child, descendant);
        if (child == RightOperand)
            return LeftOperand;

        return base.InheritedPredecessor(child, descendant);
    }

    public override IFlowNode Predecessor() => (IFlowNode)RightOperand;
}
