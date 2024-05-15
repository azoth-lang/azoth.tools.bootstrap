using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ExpressionBodyNode : CodeNode, IExpressionBodyNode
{
    public override IExpressionBodySyntax Syntax { get; }
    public IResultStatementNode ResultStatement { get; }

    public ExpressionBodyNode(IExpressionBodySyntax syntax, IResultStatementNode resultStatement)
    {
        Syntax = syntax;
        ResultStatement = Child.Attach(this, resultStatement);
    }
}
