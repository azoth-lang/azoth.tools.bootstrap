using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AsyncStartExpressionNode : ExpressionNode, IAsyncStartExpressionNode
{
    public override IAsyncStartExpressionSyntax Syntax { get; }
    public bool Scheduled => Syntax.Scheduled;
    public IAmbiguousExpressionNode Expression { get; }

    public AsyncStartExpressionNode(IAsyncStartExpressionSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        Expression = Child.Attach(this, expression);
    }

    internal override ISemanticNode? InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant == Expression) return Predecessor();
        return base.InheritedPredecessor(child, descendant);
    }
}
