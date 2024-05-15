using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AsyncStartExpressionNode : ExpressionNode, IAsyncStartExpressionNode
{
    public override IAsyncStartExpressionSyntax Syntax { get; }
    public bool Scheduled => Syntax.Scheduled;
    public IUntypedExpressionNode Expression { get; }

    public AsyncStartExpressionNode(IAsyncStartExpressionSyntax syntax, IUntypedExpressionNode expression)
    {
        Syntax = syntax;
        Expression = Child.Attach(this, expression);
    }
}
