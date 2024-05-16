using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AwaitExpressionNode : ExpressionNode, IAwaitExpressionNode
{
    public override IAwaitExpressionSyntax Syntax { get; }
    private Child<IUntypedExpressionNode> expression;
    public IUntypedExpressionNode Expression => expression.Value;

    public AwaitExpressionNode(IAwaitExpressionSyntax syntax, IUntypedExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }
}
