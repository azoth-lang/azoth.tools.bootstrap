using System;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ChildNode : SemanticNode, IChildNode
{
    private SemanticNode? parent;
    protected SemanticNode Parent
        => parent ?? throw new InvalidOperationException(Child.ParentMissingMessage(this));
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

    protected CodeFile InheritedFile()
        => Parent.InheritedFile(this, this);

    internal override PackageNameScope InheritedPackageNameScope(IChildNode caller, IChildNode child)
        => Parent.InheritedPackageNameScope(this, child);

    protected PackageNameScope InheritedPackageNameScope()
        => Parent.InheritedPackageNameScope(this, this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode caller, IChildNode child)
        => Parent.InheritedContainingLexicalScope(this, child);

    protected LexicalScope InheritedContainingLexicalScope()
        => Parent.InheritedContainingLexicalScope(this, this);

    internal override Promise<IDeclaredUserType> InheritedContainingDeclaredType(IChildNode caller, IChildNode child)
        => Parent.InheritedContainingDeclaredType(this, child);

    protected Promise<IDeclaredUserType> InheritedContainingDeclaredType()
        => Parent.InheritedContainingDeclaredType(this, this);
}
