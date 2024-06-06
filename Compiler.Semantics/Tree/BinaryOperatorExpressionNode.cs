using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BinaryOperatorExpressionNode : ExpressionNode, IBinaryOperatorExpressionNode
{
    public override IBinaryOperatorExpressionSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> leftOperand;
    public IAmbiguousExpressionNode LeftOperand => leftOperand.Value;
    public IExpressionNode FinalLeftOperand => (IExpressionNode)leftOperand.FinalValue;
    public BinaryOperator Operator => Syntax.Operator;
    private Child<IAmbiguousExpressionNode> rightOperand;
    public IAmbiguousExpressionNode RightOperand => rightOperand.Value;
    public IExpressionNode FinalRightOperand => (IExpressionNode)rightOperand.FinalValue;
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.BinaryOperatorExpression_Antetype);

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
        if (child == LeftOperand)
            return GetContainingLexicalScope();
        if (child == RightOperand)
            return LexicalScopingAspect.BinaryOperatorExpression_InheritedContainingLexicalScope_RightOperand(this);
        throw new ArgumentException("Not a child of this node.", nameof(child));
    }

    internal override FlowState InheritedFlowStateBefore(IChildNode child, IChildNode descendant)
    {
        if (child == RightOperand)
            return ((IExpressionNode)LeftOperand).FlowStateAfter;
        return base.InheritedFlowStateBefore(child, descendant);
    }
}
