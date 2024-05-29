using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AwaitExpressionNode : ExpressionNode, IAwaitExpressionNode
{
    public override IAwaitExpressionSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> expression;
    public IAmbiguousExpressionNode Expression => expression.Value;

    public AwaitExpressionNode(IAwaitExpressionSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Expression.GetFlowLexicalScope();

    internal override IFlowNode InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant == Expression) return Predecessor();
        return base.InheritedPredecessor(child, descendant);
    }
}
