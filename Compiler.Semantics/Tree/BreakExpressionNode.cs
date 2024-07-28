using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BreakExpressionNode : ExpressionNode, IBreakExpressionNode
{
    public override IBreakExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode? Value { get; }
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
        Value = Child.Attach(this, value);
    }
}
