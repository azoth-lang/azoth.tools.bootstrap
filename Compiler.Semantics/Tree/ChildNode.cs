using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ChildNode : SemanticNode, IChildNode
{
    protected virtual bool MayHaveRewrite => false;
    bool IChild.MayHaveRewrite => MayHaveRewrite;

    private SemanticNode? parent;
    protected SemanticNode Parent
        => parent ?? throw new InvalidOperationException(Child.ParentMissingMessage(this));
    ISemanticNode IChildNode.Parent => Parent;

    public IPackageNode Package => Parent.InheritedPackage(this, this);

    void IChild<ISemanticNode>.AttachParent(ISemanticNode newParent)
    {
        if (newParent is not SemanticNode newParentNode)
            throw new ArgumentException($"Parent must be a {nameof(SemanticNode)}.", nameof(newParent));
        var oldParent = Interlocked.CompareExchange(ref parent, newParentNode, null);
        if (oldParent is not null)
            throw new InvalidOperationException("Parent is already set.");
    }

    protected virtual IChildNode Rewrite()
        => RewriteNotSupported<IChildNode>();

    IChild IChild.Rewrite() => Rewrite();

    [DoesNotReturn]
    protected T RewriteNotSupported<T>()
        where T : IChildNode
        => throw new NotSupportedException(Child.RewriteNotSupportedMessaged(this));

    internal override ISymbolNode InheritedContainingSymbolNode(IChildNode caller, IChildNode child)
        => Parent.InheritedContainingSymbolNode(this, child);

    protected ISymbolNode InheritedContainingSymbolNode()
        => Parent.InheritedContainingSymbolNode(this, this);

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

    internal override IDeclaredUserType InheritedContainingDeclaredType(IChildNode caller, IChildNode child)
        => Parent.InheritedContainingDeclaredType(this, child);

    protected virtual IDeclaredUserType InheritedContainingDeclaredType()
        => Parent.InheritedContainingDeclaredType(this, this);

    internal override Pseudotype? InheritedSelfType(IChildNode caller, IChildNode child)
        => Parent.InheritedSelfType(this, child);

    protected Pseudotype? InheritedSelfType()
        => Parent.InheritedSelfType(this, this);

    internal override ITypeDeclarationNode InheritedContainingTypeDeclaration(IChildNode caller, IChildNode child)
        => Parent.InheritedContainingTypeDeclaration(this, child);

    protected ITypeDeclarationNode InheritedContainingTypeDeclaration()
        => Parent.InheritedContainingTypeDeclaration(this, this);

    internal override bool InheritedIsAttributeType(IChildNode caller, IChildNode child)
        => Parent.InheritedIsAttributeType(this, child);

    protected bool InheritedIsAttributeType()
        => Parent.InheritedIsAttributeType(this, this);
}
