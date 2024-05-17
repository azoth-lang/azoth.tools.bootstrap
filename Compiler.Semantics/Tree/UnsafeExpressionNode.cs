using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class UnsafeExpressionNode : ExpressionNode, IUnsafeExpressionNode
{
    public override IUnsafeExpressionSyntax Syntax { get; }
    public IUntypedExpressionNode Expression { get; }

    public UnsafeExpressionNode(IUnsafeExpressionSyntax syntax, IUntypedExpressionNode expression)
    {
        Syntax = syntax;
        Expression = Child.Attach(this, expression);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Expression.GetFlowLexicalScope();
}
