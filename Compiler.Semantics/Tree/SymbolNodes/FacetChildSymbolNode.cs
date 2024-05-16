using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class FacetChildSymbolNode : ChildSymbolNode, IFacetChildDeclarationNode
{
    public IPackageFacetDeclarationNode Facet => Parent.InheritedFacet(this, this);
    public virtual StandardName? Name => (StandardName?)Symbol.Name;
}
