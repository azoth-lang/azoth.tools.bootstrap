using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

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
}
