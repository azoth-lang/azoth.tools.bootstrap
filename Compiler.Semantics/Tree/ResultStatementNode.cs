using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ResultStatementNode : StatementNode, IResultStatementNode
{
    public override IResultStatementSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> expression;
    public IAmbiguousExpressionNode Expression => expression.Value;
    public override FlowState FlowStateAfter => ((IExpressionNode)Expression).FlowStateAfter;

    public ResultStatementNode(IResultStatementSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }

    public override LexicalScope GetLexicalScope() => InheritedContainingLexicalScope();
}
