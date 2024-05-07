using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticDeclarationSymbolNode : SemanticChildSymbolNode, IDeclarationSymbolNode
{
    public abstract StandardName Name { get; }
    public IPackageFacetSymbolNode Facet => Parent.InheritedFacet(this, this);
    public abstract IEnumerable<IDeclarationSymbolNode> MembersNamed(StandardName named);
}
