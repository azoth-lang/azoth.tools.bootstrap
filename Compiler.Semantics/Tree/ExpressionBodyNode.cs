using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ExpressionBodyNode : CodeNode, IExpressionBodyNode
{
    public override IExpressionBodySyntax Syntax { get; }

    public ExpressionBodyNode(IExpressionBodySyntax syntax)
    {
        Syntax = syntax;
    }
}
