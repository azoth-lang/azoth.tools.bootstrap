using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AssignmentExpressionNode : ExpressionNode, IAssignmentExpressionNode
{
    public override IAssignmentExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousAssignableExpressionNode> leftOperand;
    private bool leftOperandCached;
    public IAmbiguousAssignableExpressionNode TempLeftOperand
        => GrammarAttribute.IsCached(in leftOperandCached) ? leftOperand.UnsafeValue
            : this.RewritableChild(ref leftOperandCached, ref leftOperand);
    public IAmbiguousAssignableExpressionNode CurrentLeftOperand => leftOperand.UnsafeValue;
    public IAssignableExpressionNode? LeftOperand => TempLeftOperand as IAssignableExpressionNode;
    public AssignmentOperator Operator => Syntax.Operator;
    private RewritableChild<IAmbiguousExpressionNode> rightOperand;
    private bool rightOperandCached;
    public IAmbiguousExpressionNode TempRightOperand
        => GrammarAttribute.IsCached(in rightOperandCached) ? rightOperand.UnsafeValue
            : this.RewritableChild(ref rightOperandCached, ref rightOperand);
    public IAmbiguousExpressionNode CurrentRightOperand => rightOperand.UnsafeValue;
    public IExpressionNode? RightOperand => TempRightOperand as IExpressionNode;
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.AssignmentExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.AssignmentExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.AssignmentExpression_FlowStateAfter);
    private IFixedSet<IDataFlowNode>? dataFlowPrevious;
    private bool dataFlowPreviousCached;
    public IFixedSet<IDataFlowNode> DataFlowPrevious
        => GrammarAttribute.IsCached(in dataFlowPreviousCached) ? dataFlowPrevious!
            : this.Synthetic(ref dataFlowPreviousCached, ref dataFlowPrevious,
                DataFlowAspect.DataFlow_DataFlowPrevious, FixedSet.ObjectEqualityComparer);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyAssigned = Circular.Unset;
    private bool definitelyAssignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned
        => GrammarAttribute.IsCached(in definitelyAssignedCached) ? definitelyAssigned.UnsafeValue
            : this.Circular(ref definitelyAssignedCached, ref definitelyAssigned,
                DefiniteAssignmentAspect.AssignmentExpression_DefinitelyAssigned,
                DefiniteAssignmentAspect.DataFlow_DefinitelyAssigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyUnassigned = Circular.Unset;
    private bool definitelyUnassignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned
        => GrammarAttribute.IsCached(in definitelyUnassignedCached) ? definitelyUnassigned.UnsafeValue
            : this.Circular(ref definitelyUnassignedCached, ref definitelyUnassigned,
                SingleAssignmentAspect.AssignmentExpression_DefinitelyUnassigned,
                SingleAssignmentAspect.DataFlow_DefinitelyUnassigned_Initial);

    public AssignmentExpressionNode(
        IAssignmentExpressionSyntax syntax,
        IAmbiguousAssignableExpressionNode leftOperand,
        IAmbiguousExpressionNode rightOperand)
    {
        Syntax = syntax;
        this.leftOperand = Child.Create(this, leftOperand);
        this.rightOperand = Child.Create(this, rightOperand);
    }

    public override ConditionalLexicalScope FlowLexicalScope()
        => LexicalScopingAspect.AssignmentExpression_FlowLexicalScope(this);

    internal override IFlowState Inherited_FlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == CurrentRightOperand) return LeftOperand?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == CurrentRightOperand) return LeftOperand?.Antetype;
        return base.Inherited_ExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == CurrentRightOperand) return LeftOperand?.Type;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.AssignmentExpression_ControlFlowNext(this);

    internal override ControlFlowSet Inherited_ControlFlowFollowing(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == CurrentLeftOperand)
            return ControlFlowSet.CreateNormal(RightOperand);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        ExpressionTypesAspect.AssignmentExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    protected override IChildNode Rewrite()
        => BindingAmbiguousNamesAspect.AssignmentExpression_Rewrite_PropertyNameLeftOperand(this)
        ?? base.Rewrite();
}
