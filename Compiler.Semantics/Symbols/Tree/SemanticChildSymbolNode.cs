using System;
using System.Threading;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticChildSymbolNode : SemanticSymbolNode, IChildSymbolNode
{
    private SemanticSymbolNode? parent;
    protected SemanticSymbolNode Parent => parent ?? throw new InvalidOperationException("Parent is not set.");
    ISymbolNode IChildSymbolNode.Parent => Parent;

    public IPackageSymbolNode Package => Parent.InheritedPackage(this, this);

    public void AttachParent(ISymbolNode newParent)
    {
        if (newParent is not SemanticSymbolNode newParentNode)
            throw new ArgumentException($"Parent must be a {nameof(SemanticSymbolNode)}.", nameof(newParent));
        var oldParent = Interlocked.CompareExchange(ref parent, newParentNode, null);
        if (oldParent is not null) throw new InvalidOperationException("Parent is already set.");
    }

    public override IPackageSymbolNode InheritedPackage(IChildSymbolNode caller, IChildSymbolNode child)
        => Parent.InheritedPackage(this, child);
}
