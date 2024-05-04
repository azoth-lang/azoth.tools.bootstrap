using System;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ChildNode : SemanticNode, IChildNode
{
    private SemanticNode? parent;
    protected SemanticNode Parent => parent ?? throw new InvalidOperationException("Parent is not set.");
    ISemanticNode IChildNode.Parent => Parent;

    public IPackageNode Package => Parent.InheritedPackage(this, this);

    public void AttachParent(ISemanticNode newParent)
    {
        if (newParent is not SemanticNode newParentNode)
            throw new ArgumentException($"Parent must be a {nameof(SemanticNode)}.", nameof(newParent));
        var oldParent = Interlocked.CompareExchange(ref parent, newParentNode, null);
        if (oldParent is not null)
            throw new InvalidOperationException("Parent is already set.");
    }

    internal override ISymbolNode InheritedContainingSymbolNode(IChildNode caller, IChildNode child)
        => Parent.InheritedContainingSymbolNode(this, child);

    internal override IPackageNode InheritedPackage(IChildNode caller, IChildNode child)
        => Parent.InheritedPackage(this, child);

    internal override CodeFile InheritedFile(IChildNode caller, IChildNode child)
        => Parent.InheritedFile(this, child);

    internal override LexicalScope InheritedLexicalScope(IChildNode caller, IChildNode child)
        => Parent.InheritedLexicalScope(this, child);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode caller, IChildNode child)
        => Parent.InheritedContainingLexicalScope(this, child);
}
