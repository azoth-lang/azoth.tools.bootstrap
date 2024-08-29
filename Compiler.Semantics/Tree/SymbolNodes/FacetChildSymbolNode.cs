using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class PackageFacetChildSymbolNode : ChildSymbolNode, IPackageFacetChildDeclarationNode
{
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public virtual StandardName? Name => (StandardName?)Symbol.Name;
}
