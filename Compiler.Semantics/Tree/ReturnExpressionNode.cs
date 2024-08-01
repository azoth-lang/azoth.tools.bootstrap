using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ReturnExpressionNode : ExpressionNode, IReturnExpressionNode
{
    public override IReturnExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode?> value;
    private bool valueCached;
    public IAmbiguousExpressionNode? Value
        => GrammarAttribute.IsCached(in valueCached) ? value.UnsafeValue
            : this.RewritableChild(ref valueCached, ref value);
    public IExpressionNode? IntermediateValue => Value as IExpressionNode;
    public override IMaybeExpressionAntetype Antetype => IAntetype.Never;
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
}
