using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MoveVariableExpressionNode : ExpressionNode, IMoveVariableExpressionNode
{
    public override IExpressionSyntax Syntax { get; }
    private RewritableChild<ILocalBindingNameExpressionNode> referent;
    private bool referentCached;
    public ILocalBindingNameExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public bool IsImplicit { get; }
    public override IMaybeExpressionAntetype Antetype => Referent.Antetype;
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.MoveExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.MoveVariableExpression_FlowStateAfter);

    public MoveVariableExpressionNode(
        IExpressionSyntax syntax,
        ILocalBindingNameExpressionNode referent,
        bool isImplicit)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        IsImplicit = isImplicit;
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        ExpressionTypesAspect.MoveVariableExpression_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.MoveExpression_ControlFlowNext(this);
}
