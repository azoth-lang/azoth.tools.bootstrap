using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ChildNode : SemanticNode, IChildNode
{
    private bool inFinalTree;

    /// <remarks>Volatile read not necessary because an out-of-order read is not an issue since it
    /// will just re-figure out the fact that the node is final.</remarks>
    protected sealed override bool InFinalTree => inFinalTree;

    protected virtual bool MayHaveRewrite => false;
    bool IChildTreeNode.MayHaveRewrite => MayHaveRewrite;

    private SemanticNode? parent;

    protected SemanticNode Parent
    {
        [DebuggerStepThrough]
        // Use volatile read to ensure order of operations as seen by other threads
        get => Volatile.Read(in parent) ?? throw new InvalidOperationException(Child.ParentMissingMessage(this));
    }

    [DebuggerStepThrough]
    protected sealed override ITreeNode PeekParent()
        // Use volatile read to ensure order of operations as seen by other threads
        => Volatile.Read(in parent) ?? throw new InvalidOperationException(Child.ParentMissingMessage(this));

    protected SemanticNode GetParent(IInheritanceContext ctx)
    {
        // Use volatile read to ensure order of operations as seen by other threads
        var node = Volatile.Read(in parent) ?? throw new InvalidOperationException(Child.ParentMissingMessage(this));
        ctx.AccessParentOf(this);
        return node;
    }

    // TODO this should only be available in the final tree
    ISemanticNode IChildNode.Parent => Parent;

    public IPackageDeclarationNode Package => Parent.InheritedPackage(this, this);

    private protected ChildNode()
    {
    }

    void IChildTreeNode<ISemanticNode>.SetParent(ISemanticNode newParent)
    {
        if (newParent is not SemanticNode newParentNode)
            throw new ArgumentException($"Parent must be a {nameof(SemanticNode)}.", nameof(newParent));

        // Use volatile write to ensure order of operations as seen by other threads
        Volatile.Write(ref parent, newParentNode);
    }

    protected virtual IChildNode? Rewrite() => MayHaveRewrite ? this : throw Child.RewriteNotSupported(this);
    // TODO remove call to AttachRewritten once it is all handled by GrammarAttribute
    IChildTreeNode? IChildTreeNode.Rewrite() => Child.AttachRewritten(Parent, Rewrite());

    /// <remarks>Volatile write not necessary because an out-of-order read is not an issue since it
    /// will just re-figure out the fact that the node is final. Does not check the invariant that
    /// the parent is in the final tree because that would probably require a volatile read and that
    /// volatile was used in other places too.</remarks>
    protected sealed override void MarkInFinalTree()
        => inFinalTree = true;

    /// <summary>
    /// The previous node to this one in a preorder traversal of the tree.
    /// </summary>
    protected virtual SemanticNode Previous(IInheritanceContext ctx)
    {
        SemanticNode? previous = null;
        var parent = GetParent(ctx);
        foreach (var child in parent.Children().Cast<SemanticNode>())
        {
            if (child == this)
                // If this is the first child, return the parent without descending
                return previous?.LastDescendant() ?? parent;
            previous = child;
        }

        throw new UnreachableException("Node is not a child of its parent.");
    }

    internal override ISymbolDeclarationNode InheritedContainingDeclaration(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedContainingDeclaration(this, descendant, ctx);

    protected ISymbolDeclarationNode InheritedContainingDeclaration(IInheritanceContext ctx)
        => GetParent(ctx).InheritedContainingDeclaration(this, this, ctx);

    internal override IPackageDeclarationNode InheritedPackage(IChildNode child, IChildNode descendant)
        => Parent.InheritedPackage(this, descendant);

    internal override CodeFile InheritedFile(IChildNode child, IChildNode descendant)
        => Parent.InheritedFile(this, descendant);

    protected CodeFile InheritedFile() => Parent.InheritedFile(this, this);

    internal override PackageNameScope InheritedPackageNameScope(IChildNode child, IChildNode descendant)
        => Parent.InheritedPackageNameScope(this, descendant);

    protected PackageNameScope InheritedPackageNameScope() => Parent.InheritedPackageNameScope(this, this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedContainingLexicalScope(this, descendant, ctx);

    protected LexicalScope InheritedContainingLexicalScope(IInheritanceContext ctx)
        => GetParent(ctx).InheritedContainingLexicalScope(this, this, ctx);

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

    internal override IFlowState InheritedFlowStateBefore(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedFlowStateBefore(this, descendant, ctx);

    protected IFlowState InheritedFlowStateBefore(IInheritanceContext ctx)
        => GetParent(ctx).InheritedFlowStateBefore(this, this, ctx);

    internal override IMaybeAntetype InheritedBindingAntetype(IChildNode child, IChildNode descendant)
        => Parent.InheritedBindingAntetype(this, descendant);

    protected IMaybeAntetype InheritedBindingAntetype()
        => Parent.InheritedBindingAntetype(this, this);

    internal override DataType InheritedBindingType(IChildNode child, IChildNode descendant)
        => Parent.InheritedBindingType(this, descendant);

    protected DataType InheritedBindingType()
        => Parent.InheritedBindingType(this, this);

    internal override ValueId? InheritedMatchReferentValueId(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedMatchReferentValueId(this, descendant, ctx);

    protected ValueId? InheritedMatchReferentValueId(IInheritanceContext ctx)
        => GetParent(ctx).InheritedMatchReferentValueId(this, this, ctx);

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedExpectedAntetype(this, descendant, ctx);

    protected IMaybeExpressionAntetype? InheritedExpectedAntetype(IInheritanceContext ctx)
        => GetParent(ctx).InheritedExpectedAntetype(this, this, ctx);

    internal override DataType? InheritedExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedExpectedType(this, descendant, ctx);

    protected DataType? InheritedExpectedType(IInheritanceContext ctx)
        => GetParent(ctx).InheritedExpectedType(this, this, ctx);

    internal override DataType? InheritedExpectedReturnType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedExpectedReturnType(this, descendant, ctx);

    protected DataType? InheritedExpectedReturnType(IInheritanceContext ctx)
        => GetParent(ctx).InheritedExpectedReturnType(this, this, ctx);

    internal override ControlFlowSet InheritedControlFlowFollowing(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedControlFlowFollowing(this, descendant, ctx);

    protected ControlFlowSet InheritedControlFlowFollowing(IInheritanceContext ctx)
        => GetParent(ctx).InheritedControlFlowFollowing(this, this, ctx);

    internal override FixedDictionary<IVariableBindingNode, int> InheritedVariableBindingsMap(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedVariableBindingsMap(this, descendant, ctx);

    protected FixedDictionary<IVariableBindingNode, int> InheritedLocalBindingsMap(IInheritanceContext ctx)
        => GetParent(ctx).InheritedVariableBindingsMap(this, this, ctx);

    internal override IEntryNode InheritedControlFlowEntry(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedControlFlowEntry(this, descendant, ctx);

    protected IEntryNode InheritedControlFlowEntry(IInheritanceContext ctx)
        => GetParent(ctx).InheritedControlFlowEntry(this, this, ctx);

    internal override IExitNode InheritedControlFlowExit(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedControlFlowExit(this, descendant, ctx);

    protected IExitNode InheritedControlFlowExit(IInheritanceContext ctx)
        => GetParent(ctx).InheritedControlFlowExit(this, this, ctx);

    internal override bool InheritedImplicitRecoveryAllowed(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedImplicitRecoveryAllowed(this, descendant, ctx);

    protected bool InheritedImplicitRecoveryAllowed(IInheritanceContext ctx)
        => GetParent(ctx).InheritedImplicitRecoveryAllowed(this, this, ctx);

    internal override IPreviousValueId PreviousValueId(IChildNode before, IInheritanceContext ctx)
        => Previous(ctx).PreviousValueId(before, ctx);

    protected IPreviousValueId PreviousValueId(IInheritanceContext ctx)
        => Previous(ctx).PreviousValueId(this, ctx);

    internal override ControlFlowSet CollectControlFlowPrevious(IControlFlowNode target, IInheritanceContext ctx)
    {
        if (this is IExecutableDefinitionNode)
            return base.CollectControlFlowPrevious(target, ctx);
        return GetParent(ctx).CollectControlFlowPrevious(target, ctx);
    }
}
