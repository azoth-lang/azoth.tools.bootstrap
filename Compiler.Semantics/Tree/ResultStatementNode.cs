using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ResultStatementNode : CodeNode, IResultStatementNode
{
    public override IResultStatementSyntax Syntax { get; }
    private Child<IUntypedExpressionNode> expression;
    public IUntypedExpressionNode Expression => expression.Value;

    public ResultStatementNode(IResultStatementSyntax syntax, IUntypedExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }
}