using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class AmbiguousExpressionNode : CodeNode, IAmbiguousExpressionNode
{
    public abstract override IExpressionSyntax Syntax { get; }

    protected override IAmbiguousExpressionNode? Rewrite()
        => throw Child.RewriteNotSupported(this);

    public LexicalScope GetContainingLexicalScope() => InheritedContainingLexicalScope();

    public virtual ConditionalLexicalScope GetFlowLexicalScope()
        => LexicalScopingAspect.UntypedExpression_GetFlowLexicalScope(this);
}
