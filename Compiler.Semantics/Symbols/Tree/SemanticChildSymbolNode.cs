using System;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticChildSymbolNode : SemanticSymbolNode, IChildSymbolNode
{
    protected virtual bool MayHaveRewrite => false;
    bool IChild.MayHaveRewrite => MayHaveRewrite;

    private SemanticSymbolNode? parent;
    protected SemanticSymbolNode Parent
        => parent ?? throw new InvalidOperationException(Child.ParentMissingMessage(this));
    ISymbolNode IChildSymbolNode.Parent => Parent;

    public IPackageSymbolNode Package => Parent.InheritedPackage(this, this);

    void IChild<ISymbolNode>.AttachParent(ISymbolNode newParent)
    {
        if (newParent is not SemanticSymbolNode newParentNode)
            throw new ArgumentException($"Parent must be a {nameof(SemanticSymbolNode)}.", nameof(newParent));
        var oldParent = Interlocked.CompareExchange(ref parent, newParentNode, null);
        if (oldParent is not null) throw new InvalidOperationException("Parent is already set.");
    }

    protected IChild Rewrite()
        => throw new NotSupportedException(Child.RewriteNotSupportedMessaged(this));
    IChild IChild.Rewrite() => Rewrite();

    internal override IPackageSymbolNode InheritedPackage(IChildSymbolNode caller, IChildSymbolNode child)
        => Parent.InheritedPackage(this, child);

    internal override IPackageFacetSymbolNode InheritedFacet(IChildSymbolNode caller, IChildSymbolNode child)
        => Parent.InheritedFacet(this, child);
}
