using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
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
        get => Volatile.Read(in parent) ?? throw Child.ParentMissing(this);
    }

    [DebuggerStepThrough]
    protected sealed override SemanticNode PeekParent()
        // Use volatile read to ensure order of operations as seen by other threads
        => Volatile.Read(in parent) ?? throw Child.ParentMissing(this);

    protected SemanticNode GetParent(IInheritanceContext ctx)
    {
        // Use volatile read to ensure order of operations as seen by other threads
        var node = Volatile.Read(in parent) ?? throw Child.ParentMissing(this);
        ctx.AccessParentOf(this);
        return node;
    }

    // TODO this should only be available in the final tree
    ISemanticNode IChildNode.Parent => Parent;

    public IPackageDeclarationNode Package => Parent.Inherited_Package(this, this);

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

    protected virtual IChildNode Rewrite() => MayHaveRewrite ? this : throw Child.RewriteNotSupported(this);
    IChildTreeNode IChildTreeNode.Rewrite() => Rewrite();

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

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ContainingDeclaration(this, descendant, ctx);

    protected ISymbolDeclarationNode Inherited_ContainingDeclaration(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ContainingDeclaration(this, this, ctx);

    internal override IPackageDeclarationNode Inherited_Package(IChildNode child, IChildNode descendant)
        => Parent.Inherited_Package(this, descendant);

    internal override CodeFile Inherited_File(IChildNode child, IChildNode descendant)
        => Parent.Inherited_File(this, descendant);

    protected CodeFile Inherited_File() => Parent.Inherited_File(this, this);

    internal override PackageNameScope Inherited_PackageNameScope(IChildNode child, IChildNode descendant)
        => Parent.Inherited_PackageNameScope(this, descendant);

    protected PackageNameScope Inherited_PackageNameScope() => Parent.Inherited_PackageNameScope(this, this);

    internal override LexicalScope Inherited_ContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ContainingLexicalScope(this, descendant, ctx);

    protected LexicalScope Inherited_ContainingLexicalScope(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ContainingLexicalScope(this, this, ctx);

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

    internal override IPackageFacetDeclarationNode Inherited_Facet(IChildNode child, IChildNode descendant)
        => Parent.Inherited_Facet(this, descendant);

    protected IPackageFacetDeclarationNode Inherited_Facet() => Parent.Inherited_Facet(this, this);

    internal override ISymbolTree InheritedSymbolTree(IChildNode child, IChildNode descendant)
        => Parent.InheritedSymbolTree(this, descendant);

    protected ISymbolTree InheritedSymbolTree()
        => Parent.InheritedSymbolTree(this, this);

    internal override IFlowState Inherited_FlowStateBefore(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).Inherited_FlowStateBefore(this, descendant, ctx);

    protected IFlowState Inherited_FlowStateBefore(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_FlowStateBefore(this, this, ctx);

    internal override IMaybeAntetype Inherited_ContextBindingAntetype(IChildNode child, IChildNode descendant)
        => Parent.Inherited_ContextBindingAntetype(this, descendant);

    protected IMaybeAntetype Inherited_ContextBindingAntetype()
        => Parent.Inherited_ContextBindingAntetype(this, this);

    internal override DataType Inherited_ContextBindingType(IChildNode child, IChildNode descendant)
        => Parent.Inherited_ContextBindingType(this, descendant);

    protected DataType Inherited_ContextBindingType()
        => Parent.Inherited_ContextBindingType(this, this);

    internal override ValueId? Inherited_MatchReferentValueId(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).Inherited_MatchReferentValueId(this, descendant, ctx);

    protected ValueId? Inherited_MatchReferentValueId(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_MatchReferentValueId(this, this, ctx);

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedExpectedAntetype(this, descendant, ctx);

    protected IMaybeExpressionAntetype? InheritedExpectedAntetype(IInheritanceContext ctx)
        => GetParent(ctx).InheritedExpectedAntetype(this, this, ctx);

    internal override DataType? InheritedExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).InheritedExpectedType(this, descendant, ctx);

    protected DataType? InheritedExpectedType(IInheritanceContext ctx)
        => GetParent(ctx).InheritedExpectedType(this, this, ctx);

    internal override DataType? Inherited_ExpectedReturnType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ExpectedReturnType(this, descendant, ctx);

    protected DataType? Inherited_ExpectedReturnType(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ExpectedReturnType(this, this, ctx);

    internal override ControlFlowSet Inherited_ControlFlowFollowing(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ControlFlowFollowing(this, descendant, ctx);

    protected ControlFlowSet Inherited_ControlFlowFollowing(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ControlFlowFollowing(this, this, ctx);

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).Inherited_VariableBindingsMap(this, descendant, ctx);

    protected FixedDictionary<IVariableBindingNode, int> InheritedLocalBindingsMap(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_VariableBindingsMap(this, this, ctx);

    internal override IEntryNode Inherited_ControlFlowEntry(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ControlFlowEntry(this, descendant, ctx);

    protected IEntryNode Inherited_ControlFlowEntry(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ControlFlowEntry(this, this, ctx);

    internal override IExitNode Inherited_ControlFlowExit(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ControlFlowExit(this, descendant, ctx);

    protected IExitNode Inherited_ControlFlowExit(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ControlFlowExit(this, this, ctx);

    internal override bool Inherited_ImplicitRecoveryAllowed(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ImplicitRecoveryAllowed(this, descendant, ctx);

    protected bool Inherited_ImplicitRecoveryAllowed(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ImplicitRecoveryAllowed(this, this, ctx);

    internal override bool Inherited_ShouldPrepareToReturn(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ShouldPrepareToReturn(this, descendant, ctx);

    protected bool Inherited_ShouldPrepareToReturn(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ShouldPrepareToReturn(this, this, ctx);

    internal override IPreviousValueId Previous_PreviousValueId(IChildNode before, IInheritanceContext ctx)
        => Previous(ctx).Previous_PreviousValueId(before, ctx);

    protected IPreviousValueId Previous_PreviousValueId(IInheritanceContext ctx)
        => Previous(ctx).Previous_PreviousValueId(this, ctx);

    internal override ControlFlowSet CollectControlFlowPrevious(IControlFlowNode target, IInheritanceContext ctx)
    {
        if (this is IExecutableDefinitionNode)
            return base.CollectControlFlowPrevious(target, ctx);
        return GetParent(ctx).CollectControlFlowPrevious(target, ctx);
    }
}
