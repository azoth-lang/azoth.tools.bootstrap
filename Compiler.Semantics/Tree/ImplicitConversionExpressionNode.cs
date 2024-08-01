using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class ImplicitConversionExpressionNode : ExpressionNode, IImplicitConversionExpressionNode
{
    public override ITypedExpressionSyntax Syntax => null!;

    private RewritableChild<IExpressionNode> referent;
    private bool referentCached;
    public IExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public ITypeNode ConvertToType { get; }
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this,
                ExpressionAntetypesAspect.ImplicitConversionExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.ImplicitConversionExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.ImplicitConversionExpression_FlowStateAfter);

    public ImplicitConversionExpressionNode(
        IExpressionNode referent,
        ITypeNode convertToType)
    {
        this.referent = Child.Create(this, referent);
        ConvertToType = Child.Attach(this, convertToType);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Referent.GetFlowLexicalScope();
}
