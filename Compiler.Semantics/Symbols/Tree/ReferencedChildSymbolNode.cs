using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class ReferencedChildSymbolNode : ReferencedSymbolNode, IChildDeclarationNode
{
    protected virtual bool MayHaveRewrite => false;

    bool IChild.MayHaveRewrite => MayHaveRewrite;

    private ReferencedSymbolNode? parent;
    protected virtual ReferencedSymbolNode Parent
        => parent ?? throw new InvalidOperationException(Child.ParentMissingMessage(this));

    IDeclarationNode IChildDeclarationNode.Parent => Parent;

    public IPackageDeclarationNode Package => Parent.InheritedPackage(this, this);

    void IChild<IDeclarationNode>.AttachParent(IDeclarationNode newParent)
    {
        if (newParent is not ReferencedSymbolNode newParentNode)
            throw new ArgumentException($"Parent must be a {nameof(ReferencedSymbolNode)}.", nameof(newParent));
        var oldParent = Interlocked.CompareExchange(ref parent, newParentNode, null);
        if (oldParent is not null)
            throw new InvalidOperationException("Parent is already set.");
    }

    protected IChild? Rewrite()
        => throw Child.RewriteNotSupported(this);
    IChild? IChild.Rewrite() => Rewrite();

    internal override IPackageDeclarationNode InheritedPackage(IChildDeclarationNode caller, IChildDeclarationNode child)
        => Parent.InheritedPackage(this, child);

    internal override ISymbolTree InheritedSymbolTree(IChildDeclarationNode caller, IChildDeclarationNode child)
        => Parent.InheritedSymbolTree(this, child);

    internal override IPackageFacetDeclarationNode InheritedFacet(IChildDeclarationNode caller, IChildDeclarationNode child)
        => Parent.InheritedFacet(this, child);

    protected IEnumerable<IChildDeclarationNode> GetMembers()
    {
        var symbolTree = Parent.InheritedSymbolTree(this, this);
        return symbolTree.GetChildrenOf(Symbol).Select(SymbolNodeAttributes.Symbol);
    }
}
