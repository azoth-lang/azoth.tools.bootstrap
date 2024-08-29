using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ReturnExpressionNode : ExpressionNode, IReturnExpressionNode
{
    public override IReturnExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode?> value;
    private bool valueCached;
    public IAmbiguousExpressionNode? TempValue
        => GrammarAttribute.IsCached(in valueCached) ? value.UnsafeValue
            : this.RewritableChild(ref valueCached, ref value);
    public IAmbiguousExpressionNode? CurrentValue => value.UnsafeValue;
    public IExpressionNode? Value => TempValue as IExpressionNode;
    public override IMaybeExpressionAntetype Antetype => IAntetype.Never;
    private DataType? expectedReturnType;
    private bool expectedReturnTypeCached;
    public DataType? ExpectedReturnType
        => GrammarAttribute.IsCached(in expectedReturnTypeCached) ? expectedReturnType
            : this.Inherited(ref expectedReturnTypeCached, ref expectedReturnType,
                Inherited_ExpectedReturnType);
    public override NeverType Type => DataType.Never;
    private IFlowState? flowStateAfter;
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter!
            : this.Synthetic(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.ReturnExpression_FlowStateAfter);

    public ReturnExpressionNode(IReturnExpressionSyntax syntax, IAmbiguousExpressionNode? value)
    {
        Syntax = syntax;
        this.value = Child.Create(this, value);
    }

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentValue) return ExpectedReturnType?.ToAntetype();
        return base.Inherited_ExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentValue) return ExpectedReturnType;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        InvalidStructureAspect.ReturnExpression_ContributeDiagnostics(this, diagnostics);
        ExpressionTypesAspect.ReturnExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.ReturnExpression_ControlFlowNext(this);

    internal override ControlFlowSet Inherited_ControlFlowFollowing(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == CurrentValue) return ControlFlowSet.CreateNormal(ControlFlowExit());
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    public IExitNode ControlFlowExit()
        => Inherited_ControlFlowExit(GrammarAttribute.CurrentInheritanceContext());

    internal override bool Inherited_ImplicitRecoveryAllowed(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentValue) return true;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentValue) return true;
        return false;
    }
}
