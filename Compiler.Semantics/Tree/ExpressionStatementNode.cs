using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ExpressionStatementNode : CodeNode, IExpressionStatementNode
{
    public override IExpressionStatementSyntax Syntax { get; }
    public IUntypedExpressionNode Expression { get; }

    public ExpressionStatementNode(IExpressionStatementSyntax syntax, IUntypedExpressionNode expression)
    {
        Syntax = syntax;
        Expression = Child.Attach(this, expression);
    }
}
