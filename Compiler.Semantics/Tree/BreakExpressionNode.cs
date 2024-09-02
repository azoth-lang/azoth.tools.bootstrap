using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BreakExpressionNode : ExpressionNode, IBreakExpressionNode
{
    public override IBreakExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode?> value;
    private bool valueCached;
    public IAmbiguousExpressionNode? TempValue
        => GrammarAttribute.IsCached(in valueCached) ? value.UnsafeValue
            : this.RewritableChild(ref valueCached, ref value);
    public IExpressionNode? Value => TempValue as IExpressionNode;
    public IAmbiguousExpressionNode? CurrentValue => value.UnsafeValue;
    public override IMaybeExpressionAntetype Antetype => IAntetype.Never;
    public override NeverType Type => DataType.Never;
    private IFlowState? flowStateAfter;
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter!
            : this.Synthetic(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.BreakExpression_FlowStateAfter);

    public BreakExpressionNode(IBreakExpressionSyntax syntax, IAmbiguousExpressionNode? value)
    {
        Syntax = syntax;
        this.value = Child.Create(this, value);
    }
}
