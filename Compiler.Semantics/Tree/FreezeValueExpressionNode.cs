using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FreezeValueExpressionNode : ExpressionNode, IFreezeValueExpressionNode
{
    public override IExpressionSyntax Syntax { get; }
    private RewritableChild<IExpressionNode> referent;
    private bool referentCached;
    public IExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public IExpressionNode CurrentReferent => referent.UnsafeValue;
    public bool IsTemporary { get; }
    public bool IsImplicit { get; }
    public override IMaybeExpressionAntetype Antetype => Referent.Antetype;
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.FreezeExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.FreezeValueExpression_FlowStateAfter);

    public FreezeValueExpressionNode(
        IExpressionSyntax syntax,
        IExpressionNode referent,
        bool isTemporary,
        bool isImplicit)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        IsTemporary = isTemporary;
        IsImplicit = isImplicit;
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.FreezeExpression_ControlFlowNext(this);

    internal override bool Inherited_ShouldPrepareToReturn(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentReferent && IsImplicit)
            // Pass along should prepare to return to the referent if this is an implicit freeze.
            return ShouldPrepareToReturn();
        return false;
    }

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        ExpressionTypesAspect.FreezeValueExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
