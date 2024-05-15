using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ResultStatementNode : CodeNode, IResultStatementNode
{
    public override IResultStatementSyntax Syntax { get; }
    public IUntypedExpressionNode Expression { get; }

    public ResultStatementNode(IResultStatementSyntax syntax, IUntypedExpressionNode expression)
    {
        Syntax = syntax;
        Expression = Child.Attach(this, expression);
    }
}
