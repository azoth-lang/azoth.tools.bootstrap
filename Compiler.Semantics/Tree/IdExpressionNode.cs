using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdExpressionNode : ExpressionNode, IIdExpressionNode
{
    public override IIdExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> referent;
    private bool referentCached;
    public IAmbiguousExpressionNode TempReferent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public IExpressionNode? Referent => TempReferent as IExpressionNode;
    public override IMaybeExpressionAntetype Antetype
        => Referent?.Antetype ?? IAntetype.Unknown;
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.IdExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.IdExpression_FlowStateAfter);

    public IdExpressionNode(IIdExpressionSyntax syntax, IAmbiguousExpressionNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }

    public override ConditionalLexicalScope FlowLexicalScope() => TempReferent.FlowLexicalScope();

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics, bool contributeAttribute = true)
    {
        ExpressionTypesAspect.IdExpression_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics, contributeAttribute);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.IdExpression_ControlFlowNext(this);
}
