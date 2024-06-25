using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ExpressionStatementNode : StatementNode, IExpressionStatementNode
{
    public override IExpressionStatementSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> expression;
    private bool expressionCached;
    public IAmbiguousExpressionNode Expression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public override IMaybeAntetype? ResultAntetype => null;
    public override DataType? ResultType => null;
    public override FlowState FlowStateAfter => ((IExpressionNode)Expression).FlowStateAfter;

    public ExpressionStatementNode(IExpressionStatementSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }

    public override LexicalScope GetLexicalScope() => InheritedContainingLexicalScope();
}
