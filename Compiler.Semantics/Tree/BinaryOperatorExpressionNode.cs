using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BinaryOperatorExpressionNode : ExpressionNode, IBinaryOperatorExpressionNode
{
    public override IBinaryOperatorExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> leftOperand;
    private bool leftOperandCached;
    public IAmbiguousExpressionNode LeftOperand
        => GrammarAttribute.IsCached(in leftOperandCached) ? leftOperand.UnsafeValue
            : this.RewritableChild(ref leftOperandCached, ref leftOperand);
    public IAmbiguousExpressionNode CurrentLeftOperand => leftOperand.UnsafeValue;
    public IExpressionNode? IntermediateLeftOperand => LeftOperand as IExpressionNode;
    public BinaryOperator Operator => Syntax.Operator;
    private RewritableChild<IAmbiguousExpressionNode> rightOperand;
    private bool rightOperandCached;
    public IAmbiguousExpressionNode RightOperand
        => GrammarAttribute.IsCached(in rightOperandCached) ? rightOperand.UnsafeValue
            : this.RewritableChild(ref rightOperandCached, ref rightOperand);
    public IAmbiguousExpressionNode CurrentRightOperand => rightOperand.UnsafeValue;
    public IExpressionNode? IntermediateRightOperand => RightOperand as IExpressionNode;
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public override LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                InheritedContainingLexicalScope, ReferenceEqualityComparer.Instance);
    private IAntetype? numericOperatorCommonAntetype;
    private bool numericOperatorCommonAntetypeCached;
    public IAntetype? NumericOperatorCommonAntetype
        => GrammarAttribute.IsCached(in numericOperatorCommonAntetypeCached) ? numericOperatorCommonAntetype
            : this.Synthetic(ref numericOperatorCommonAntetypeCached, ref numericOperatorCommonAntetype,
                ExpressionAntetypesAspect.BinaryOperatorExpression_NumericOperatorCommonAntetype);
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.BinaryOperatorExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.BinaryOperatorExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.BinaryOperatorExpression_FlowStateAfter);

    public BinaryOperatorExpressionNode(
        IBinaryOperatorExpressionSyntax syntax,
        IAmbiguousExpressionNode leftOperand,
        IAmbiguousExpressionNode rightOperand)
    {
        Syntax = syntax;
        this.leftOperand = Child.Create(this, leftOperand);
        this.rightOperand = Child.Create(this, rightOperand);
    }

    public override ConditionalLexicalScope FlowLexicalScope()
        => LexicalScopingAspect.BinaryOperatorExpression_FlowLexicalScope(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == CurrentLeftOperand)
            return ContainingLexicalScope;
        if (child == CurrentRightOperand)
            return LexicalScopingAspect.BinaryOperatorExpression_RightOperand_Broadcast_ContainingLexicalScope(this);
        throw new ArgumentException("Not a child of this node.", nameof(child));
    }

    internal override IFlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == CurrentRightOperand)
            return IntermediateLeftOperand?.FlowStateAfter ?? IFlowState.Empty;
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentLeftOperand || descendant == CurrentRightOperand)
            return NumericOperatorCommonAntetype;

        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        ExpressionTypesAspect.BinaryOperatorExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.BinaryOperatorExpression_ControlFlowNext(this);

    internal override ControlFlowSet InheritedControlFlowFollowing(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == CurrentLeftOperand) return ControlFlowSet.CreateNormal(IntermediateRightOperand);
        return base.InheritedControlFlowFollowing(child, descendant, ctx);
    }
}
