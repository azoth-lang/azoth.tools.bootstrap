using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class PatternNode : CodeNode, IPatternNode
{
    public abstract override IPatternSyntax Syntax { get; }

    private protected PatternNode() { }

    public virtual LexicalScope GetContainingLexicalScope() => InheritedContainingLexicalScope();

    public abstract ConditionalLexicalScope GetFlowLexicalScope();
}
