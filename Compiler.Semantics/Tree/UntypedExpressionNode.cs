using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class AmbiguousExpressionNode : CodeNode, IAmbiguousExpressionNode
{
    protected AttributeLock SyncLock;

    public abstract override IExpressionSyntax Syntax { get; }

    private protected AmbiguousExpressionNode() { }

    public LexicalScope GetContainingLexicalScope()
        => InheritedContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());

    public virtual ConditionalLexicalScope GetFlowLexicalScope()
        => LexicalScopingAspect.UntypedExpression_GetFlowLexicalScope(this);
}
