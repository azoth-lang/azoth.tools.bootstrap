using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class AmbiguousExpressionNode : CodeNode, IAmbiguousExpressionNode
{
    public abstract override IExpressionSyntax Syntax { get; }

    private protected AmbiguousExpressionNode() { }

    public LexicalScope GetContainingLexicalScope() => InheritedContainingLexicalScope();

    public virtual ConditionalLexicalScope GetFlowLexicalScope()
        => LexicalScopingAspect.UntypedExpression_GetFlowLexicalScope(this);

    public ValueIdScope ValueIdScope() => InheritedValueIdScope();
}
