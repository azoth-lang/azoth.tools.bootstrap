using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class LiteralExpressionNode : ExpressionNode, ILiteralExpressionNode
{
    public abstract override ILiteralExpressionSyntax Syntax { get; }
    public override FlowState FlowStateAfter
        => InheritedFlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
}
