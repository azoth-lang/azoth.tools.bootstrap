using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MoveVariableExpressionNode : ExpressionNode, IMoveVariableExpressionNode
{
    public override ITypedExpressionSyntax Syntax { get; }
    public ILocalBindingNameExpressionNode Referent { get; }
    public bool IsImplicit { get; }
    public override IMaybeExpressionAntetype Antetype => Referent.Antetype;
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.MoveExpression_Type);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.MoveVariableExpression_FlowStateAfter);

    public MoveVariableExpressionNode(
        ITypedExpressionSyntax syntax,
        ILocalBindingNameExpressionNode referent,
        bool isImplicit)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
        IsImplicit = isImplicit;
    }
}
