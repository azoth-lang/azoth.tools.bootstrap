using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ChildNode : SemanticNode, IChildNode
{
    protected virtual bool MayHaveRewrite => false;
    bool IChild.MayHaveRewrite => MayHaveRewrite;

    private bool isFinal;

    /// <remarks>Child nodes start final if <see cref="MayHaveRewrite"/> is false and their parent
    /// is final. Otherwise, they start non-final and must be marked final via the rewrite system.</remarks>
    public override bool IsFinal => Volatile.Read(in isFinal);

    private SemanticNode? parent;

    protected SemanticNode Parent
    {
        [DebuggerStepThrough]
        // Use volatile read to ensure order of operations as seen by other threads
        get => Volatile.Read(in parent) ?? throw new InvalidOperationException(Child.ParentMissingMessage(this));
    }

    protected SemanticNode GetParent(IInheritanceContext ctx)
    {
        // If this node isn't final, then the parent could be changed by a rewrite. In which case,
        // attributes depending on the parent must not be cached.
        if (!IsFinal)
            ctx.MarkNonFinal();
        // Use volatile read to ensure order of operations as seen by other threads
        return Volatile.Read(in parent) ?? throw new InvalidOperationException(Child.ParentMissingMessage(this));
    }

    ISemanticNode IChildNode.Parent => Parent;

    public IPackageDeclarationNode Package => Parent.InheritedPackage(this, this);

    private protected ChildNode()
    {
    }

    void IChild<ISemanticNode>.SetParent(ISemanticNode newParent)
    {
        if (newParent is not SemanticNode newParentNode)
            throw new ArgumentException($"Parent must be a {nameof(SemanticNode)}.", nameof(newParent));

        // Use volatile write to ensure order of operations as seen by other threads
        Volatile.Write(ref parent, newParentNode);

        if (newParentNode.IsFinal && !MayHaveRewrite)
            Volatile.Write(ref isFinal, true);

        // TODO if we could recur and propagate the final state down the tree, that would be better.
        // but we don't want to access the children and cause then to start rewriting. We would need
        // a way of getting the current children without causing them to rewrite.
    }

    protected virtual IChildNode? Rewrite() => throw Child.RewriteNotSupported(this);

    IChild? IChild.Rewrite() => Child.AttachRewritten(Parent, Rewrite());

    void IChild.MarkFinal() => Volatile.Write(ref isFinal, true);

    /// <summary>
    /// The previous node to this one in a preorder traversal of the tree.
    /// </summary>
    protected virtual SemanticNode Previous()
    {
        SemanticNode? previous = null;
        foreach (var child in Parent.Children().Cast<SemanticNode>())
        {
            if (child == this)
                // If this is the first child, return the parent without descending
                return previous?.LastDescendant() ?? Parent;
            previous = child;
        }

        throw new UnreachableException("Node is not a child of its parent.");
    }

    internal override ISymbolDeclarationNode InheritedContainingDeclaration(IChildNode child, IChildNode descendant)
        => Parent.InheritedContainingDeclaration(this, descendant);

    protected ISymbolDeclarationNode InheritedContainingDeclaration()
        => Parent.InheritedContainingDeclaration(this, this);

    internal override IPackageDeclarationNode InheritedPackage(IChildNode child, IChildNode descendant)
        => Parent.InheritedPackage(this, descendant);

    internal override CodeFile InheritedFile(IChildNode child, IChildNode descendant)
        => Parent.InheritedFile(this, descendant);

    protected CodeFile InheritedFile() => Parent.InheritedFile(this, this);

    internal override PackageNameScope InheritedPackageNameScope(IChildNode child, IChildNode descendant)
        => Parent.InheritedPackageNameScope(this, descendant);

    protected PackageNameScope InheritedPackageNameScope() => Parent.InheritedPackageNameScope(this, this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
        => Parent.InheritedContainingLexicalScope(this, descendant);

    protected LexicalScope InheritedContainingLexicalScope() => Parent.InheritedContainingLexicalScope(this, this);

    internal override IDeclaredUserType InheritedContainingDeclaredType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => Parent.InheritedContainingDeclaredType(this, descendant, ctx);

    protected virtual IDeclaredUserType InheritedContainingDeclaredType(IInheritanceContext ctx)
        => GetParent(ctx).InheritedContainingDeclaredType(this, this, ctx);

    internal override Pseudotype? InheritedSelfType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedSelfType(this, descendant, ctx);

    protected Pseudotype? InheritedSelfType(IInheritanceContext ctx)
        => GetParent(ctx).InheritedSelfType(this, this, ctx);

    internal override ITypeDefinitionNode InheritedContainingTypeDefinition(IChildNode child, IChildNode descendant)
        => Parent.InheritedContainingTypeDefinition(this, descendant);

    protected ITypeDefinitionNode InheritedContainingTypeDefinition()
        => Parent.InheritedContainingTypeDefinition(this, this);

    internal override bool InheritedIsAttributeType(IChildNode child, IChildNode descendant)
        => Parent.InheritedIsAttributeType(this, descendant);

    protected bool InheritedIsAttributeType() => Parent.InheritedIsAttributeType(this, this);

    internal override IPackageFacetDeclarationNode InheritedFacet(IChildNode child, IChildNode descendant)
        => Parent.InheritedFacet(this, descendant);

    protected IPackageFacetDeclarationNode InheritedFacet() => Parent.InheritedFacet(this, this);

    internal override ISymbolTree InheritedSymbolTree(IChildNode child, IChildNode descendant)
        => Parent.InheritedSymbolTree(this, descendant);

    protected ISymbolTree InheritedSymbolTree()
        => Parent.InheritedSymbolTree(this, this);

    internal override FlowState InheritedFlowStateBefore(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedFlowStateBefore(this, descendant, ctx);

    protected FlowState InheritedFlowStateBefore(IInheritanceContext ctx)
        => GetParent(ctx).InheritedFlowStateBefore(this, this, ctx);

    internal override IMaybeAntetype InheritedBindingAntetype(IChildNode child, IChildNode descendant)
        => Parent.InheritedBindingAntetype(this, descendant);

    protected IMaybeAntetype InheritedBindingAntetype()
        => Parent.InheritedBindingAntetype(this, this);

    internal override DataType InheritedBindingType(IChildNode child, IChildNode descendant)
        => Parent.InheritedBindingType(this, descendant);

    protected DataType InheritedBindingType()
        => Parent.InheritedBindingType(this, this);

    internal override IPreviousValueId PreviousValueId(IChildNode before)
        => Previous().PreviousValueId(before);

    protected IPreviousValueId PreviousValueId()
        => Previous().PreviousValueId(this);
}
