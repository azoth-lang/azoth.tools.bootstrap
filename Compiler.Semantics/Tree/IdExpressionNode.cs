using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdExpressionNode : ExpressionNode, IIdExpressionNode
{
    public override IIdExpressionSyntax Syntax { get; }
    private IAmbiguousExpressionNode referent;
    public IAmbiguousExpressionNode Referent
        => GrammarAttribute.IsFinal(referent) ? referent
            : GrammarAttribute.Child(this, ref referent);
    public IExpressionNode? IntermediateReferent => Referent as IExpressionNode;
    public override IMaybeExpressionAntetype Antetype
        => IntermediateReferent?.Antetype ?? IAntetype.Unknown;
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : GrammarAttribute.Synthetic(ref typeCached, this,
                ExpressionTypesAspect.IdExpression_Type, ref type);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : GrammarAttribute.Circular(ref flowStateAfterCached, this,
                ExpressionTypesAspect.IdExpression_FlowStateAfter, ref flowStateAfter);

    public IdExpressionNode(IIdExpressionSyntax syntax, IAmbiguousExpressionNode referent)
    {
        Syntax = syntax;
        this.referent = Child.AttachRewritable(this, referent);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Referent.GetFlowLexicalScope();

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        ExpressionTypesAspect.IdExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
