using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class ReferencedChildSymbolNode : ReferencedSymbolNode, IChildSymbolNode
{
    private ReferencedSymbolNode? parent;
    protected virtual ReferencedSymbolNode Parent => parent ?? throw new InvalidOperationException("Parent is not set.");
    ISymbolNode IChildSymbolNode.Parent => Parent;

    public IPackageSymbolNode Package => Parent.InheritedPackage(this, this);

    public void AttachParent(ISymbolNode newParent)
    {
        if (newParent is not ReferencedSymbolNode newParentNode)
            throw new ArgumentException($"Parent must be a {nameof(ReferencedSymbolNode)}.", nameof(newParent));
        var oldParent = Interlocked.CompareExchange(ref parent, newParentNode, null);
        if (oldParent is not null)
            throw new InvalidOperationException("Parent is already set.");
    }

    internal override IPackageSymbolNode InheritedPackage(IChildSymbolNode caller, IChildSymbolNode child)
        => base.InheritedPackage(this, child);

    internal override ISymbolTree InheritedSymbolTree(IChildSymbolNode caller, IChildSymbolNode child)
        => base.InheritedSymbolTree(this, child);

    internal override IFacetSymbolNode InheritedFacet(IChildSymbolNode caller, IChildSymbolNode child)
        => Parent.InheritedFacet(this, child);

    protected IEnumerable<IChildSymbolNode> GetMembers()
    {
        var symbolTree = Parent.InheritedSymbolTree(this, this);
        return symbolTree.GetChildrenOf(Symbol).Select(SymbolNodeAttribute.Symbol);
    }
}
