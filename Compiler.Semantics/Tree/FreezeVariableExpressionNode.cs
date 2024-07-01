using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FreezeVariableExpressionNode : ExpressionNode, IFreezeVariableExpressionNode
{
    public override ITypedExpressionSyntax Syntax { get; }
    public ILocalBindingNameExpressionNode Referent { get; }
    public bool IsTemporary { get; }
    public bool IsImplicit { get; }
    public override IMaybeExpressionAntetype Antetype => Referent.Antetype;
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.FreezeExpression_Type);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.FreezeVariableExpression_FlowStateAfter);

    public FreezeVariableExpressionNode(
        ITypedExpressionSyntax syntax,
        ILocalBindingNameExpressionNode referent,
        bool isTemporary,
        bool isImplicit)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
        IsTemporary = isTemporary;
        IsImplicit = isImplicit;
    }
}
