using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PrepareToReturnExpressionNode : ExpressionNode, IPrepareToReturnExpressionNode
{
    public override IExpressionSyntax Syntax { get; }
    private RewritableChild<IExpressionNode> value;
    private bool valueCached;
    public IExpressionNode Value
        => GrammarAttribute.IsCached(in valueCached) ? value.UnsafeValue
            : this.RewritableChild(ref valueCached, ref value);
    public IExpressionNode CurrentValue => value.UnsafeValue;
    public override IMaybeExpressionAntetype Antetype => Value.Antetype;
    public override DataType Type => Value.Type;
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.PrepareToReturnExpression_FlowStateAfter);

    public PrepareToReturnExpressionNode(IExpressionNode value)
    {
        Syntax = (IExpressionSyntax)value.Syntax;
        this.value = Child.Create(this, value);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.PrepareToReturnExpression_ControlFlowNext(this);
}
