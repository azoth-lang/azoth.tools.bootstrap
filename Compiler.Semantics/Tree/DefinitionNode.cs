using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class DefinitionNode : CodeNode, IDefinitionNode
{
    public abstract override IDefinitionSyntax? Syntax { get; }
    private ValueAttribute<IPackageFacetDeclarationNode> facet;
    public IPackageFacetDeclarationNode Facet
        => facet.TryGetValue(out var value) ? value : facet.GetValue(InheritedFacet);
    public virtual IDeclarationNode ContainingDeclarationNode
        => Parent.InheritedContainingDeclaration(this, this);
    public virtual Symbol ContainingSymbol => ContainingDeclarationNode.Symbol;
    private ValueAttribute<LexicalScope> containingLexicalScope;
    public virtual LexicalScope ContainingLexicalScope
        => containingLexicalScope.TryGetValue(out var value) ? value
            : containingLexicalScope.GetValue(InheritedContainingLexicalScope);
    public abstract LexicalScope LexicalScope { get; }
}
