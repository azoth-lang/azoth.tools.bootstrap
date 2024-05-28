using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class StatementNode : CodeNode, IStatementNode
{
    public abstract override IStatementSyntax Syntax { get; }

    private protected StatementNode() { }

    public abstract LexicalScope GetLexicalScope();

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
        => GetLexicalScope();

    public IExpressionNode? Predecessor() => (IExpressionNode?)InheritedPredecessor();

    public abstract IExpressionNode? LastExpression();
}
