using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BindingPatternNode : PatternNode, IBindingPatternNode
{
    public override IBindingPatternSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IdentifierName Name => Syntax.Name;
    private ValueAttribute<LexicalScope> containingLexicalScope;
    public LexicalScope ContainingLexicalScope
        => containingLexicalScope.TryGetValue(out var value) ? value
            : containingLexicalScope.GetValue(InheritedContainingLexicalScope);

    public BindingPatternNode(IBindingPatternSyntax syntax)
    {
        Syntax = syntax;
    }

    public override LexicalScope GetContainingLexicalScope() => ContainingLexicalScope;

    public override ConditionalLexicalScope GetFlowLexicalScope()
    {
        var containingLexicalScope = ContainingLexicalScope;
        var variableScope = new DeclarationScope(containingLexicalScope, this);
        return new(variableScope, containingLexicalScope);
    }
}
