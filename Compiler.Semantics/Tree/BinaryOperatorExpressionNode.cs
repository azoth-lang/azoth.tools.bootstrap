using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BinaryOperatorExpressionNode : ExpressionNode, IBinaryOperatorExpressionNode
{
    public override IBinaryOperatorExpressionSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> leftOperand;
    public IAmbiguousExpressionNode LeftOperand => leftOperand.Value;
    public BinaryOperator Operator => Syntax.Operator;
    private Child<IAmbiguousExpressionNode> rightOperand;
    public IAmbiguousExpressionNode RightOperand => rightOperand.Value;

    public BinaryOperatorExpressionNode(
        IBinaryOperatorExpressionSyntax syntax,
        IAmbiguousExpressionNode leftOperand,
        IAmbiguousExpressionNode rightOperand)
    {
        Syntax = syntax;
        this.leftOperand = Child.Create(this, leftOperand);
        this.rightOperand = Child.Create(this, rightOperand);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope()
        => LexicalScopingAspect.BinaryOperatorExpression_GetFlowLexicalScope(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (ReferenceEquals(child, LeftOperand))
            return GetContainingLexicalScope();
        if (ReferenceEquals(child, RightOperand))
            return LexicalScopingAspect.BinaryOperatorExpression_InheritedContainingLexicalScope_RightOperand(this);
        throw new ArgumentException("Note a child of this node.", nameof(child));
    }

    internal override ISemanticNode? InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant == LeftOperand)
            return Predecessor();
        if (descendant == RightOperand)
            return LeftOperand;
        return base.InheritedPredecessor(child, descendant);
    }
}
