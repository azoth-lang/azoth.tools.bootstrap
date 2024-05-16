using System;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticChildSymbolNode : SemanticSymbolNode, IChildDeclarationNode
{
    protected virtual bool MayHaveRewrite => false;
    bool IChild.MayHaveRewrite => MayHaveRewrite;

    private SemanticSymbolNode? parent;
    protected SemanticSymbolNode Parent
        => parent ?? throw new InvalidOperationException(Child.ParentMissingMessage(this));
    IDeclarationNode IChildDeclarationNode.Parent => Parent;

    public IPackageDeclarationNode Package => Parent.InheritedPackage(this, this);

    void IChild<IDeclarationNode>.AttachParent(IDeclarationNode newParent)
    {
        if (newParent is not SemanticSymbolNode newParentNode)
            throw new ArgumentException($"Parent must be a {nameof(SemanticSymbolNode)}.", nameof(newParent));
        var oldParent = Interlocked.CompareExchange(ref parent, newParentNode, null);
        if (oldParent is not null) throw new InvalidOperationException("Parent is already set.");
    }

    protected IChild? Rewrite()
        => throw Child.RewriteNotSupported(this);
    IChild? IChild.Rewrite() => Rewrite();

    internal override IPackageDeclarationNode InheritedPackage(IChildDeclarationNode caller, IChildDeclarationNode child)
        => Parent.InheritedPackage(this, child);

    internal override IPackageFacetDeclarationNode InheritedFacet(IChildDeclarationNode caller, IChildDeclarationNode child)
        => Parent.InheritedFacet(this, child);
}
