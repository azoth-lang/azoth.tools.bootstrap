using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticDeclarationSymbolNode : SemanticChildSymbolNode, IFacetChildDeclarationNode
{
    public abstract StandardName? Name { get; }
    public IPackageFacetDeclarationNode Facet => Parent.InheritedFacet(this, this);
}
