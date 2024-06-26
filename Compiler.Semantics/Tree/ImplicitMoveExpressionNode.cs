using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ImplicitMoveExpressionNode : ExpressionNode, IImplicitMoveExpressionNode
{
    public override ITypedExpressionSyntax Syntax { get; }
    public bool IsTemporary { get; }
    private RewritableChild<IExpressionNode> referent;
    private bool referentCached;
    public IExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public override IMaybeExpressionAntetype Antetype => Referent.Antetype;
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.ImplicitMoveExpression_Type);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.ImplicitMoveExpression_FlowStateAfter);

    public ImplicitMoveExpressionNode(ITypedExpressionSyntax syntax, bool isTemporary, IExpressionNode referent)
    {
        Syntax = syntax;
        IsTemporary = isTemporary;
        this.referent = Child.Create(this, referent);
    }
}
