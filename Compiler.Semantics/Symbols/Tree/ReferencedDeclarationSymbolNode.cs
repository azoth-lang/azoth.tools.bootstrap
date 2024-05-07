using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class ReferencedDeclarationSymbolNode : ReferencedChildSymbolNode, IDeclarationSymbolNode
{
    public IPackageFacetSymbolNode Facet => Parent.InheritedFacet(this, this);
    public virtual StandardName Name => (StandardName)Symbol.Name!;
    public abstract IEnumerable<IDeclarationSymbolNode> MembersNamed(StandardName named);
}
