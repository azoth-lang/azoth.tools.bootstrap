using System;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ChildNode : SemanticNode, IChildNode
{
    private SemanticNode? parent;
    protected SemanticNode Parent => parent ?? throw new InvalidOperationException("Parent is not set.");
    ISemanticNode IChildNode.Parent => Parent;

    public override NamespaceSymbol? InheritedContainingNamespace
        => Parent.InheritedContainingNamespace;

    public void AttachParent(ISemanticNode newParent)
    {
        if (newParent is not SemanticNode newParentNode)
            throw new ArgumentException($"Parent must be a {nameof(SemanticNode)}.", nameof(newParent));
        var oldParent = Interlocked.CompareExchange(ref parent, newParentNode, null);
        if (oldParent is not null)
            throw new InvalidOperationException("Parent is already set.");
    }
}
