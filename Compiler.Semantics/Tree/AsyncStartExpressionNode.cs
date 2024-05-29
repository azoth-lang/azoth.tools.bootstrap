using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AsyncStartExpressionNode : ExpressionNode, IAsyncStartExpressionNode
{
    public override IAsyncStartExpressionSyntax Syntax { get; }
    public bool Scheduled => Syntax.Scheduled;
    private Child<IAmbiguousExpressionNode> expression;
    public IAmbiguousExpressionNode Expression => expression.Value;

    public AsyncStartExpressionNode(IAsyncStartExpressionSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }
}
