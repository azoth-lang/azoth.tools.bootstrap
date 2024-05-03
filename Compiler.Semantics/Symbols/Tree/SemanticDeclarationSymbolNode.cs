using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticDeclarationSymbolNode : SemanticChildSymbolNode, IDeclarationSymbolNode
{
    public abstract StandardName Name { get; }
    public IFacetSymbolNode Facet => Parent.InheritedFacet(this, this);
}
