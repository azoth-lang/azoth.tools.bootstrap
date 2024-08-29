using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class SemanticNode : ISemanticNode
{
    /// <remarks>Root nodes are always in the final tree</remarks>
    protected virtual bool InFinalTree => true;
    bool ITreeNode.InFinalTree => InFinalTree;

    /// <remarks>Root nodes are already final and should never be marked final but since it is not
    /// an error to mark a node final twice, this is a no-op.</remarks>
    protected virtual void MarkInFinalTree() { }
    void ITreeNode.MarkInFinalTree() => MarkInFinalTree();

    public abstract ISyntax? Syntax { get; }

    protected virtual SemanticNode? PeekParent() => null;
    ITreeNode? ITreeNode.PeekParent() => PeekParent();

    internal SemanticNode LastDescendant()
        => ((SemanticNode?)this.Children().LastOrDefault())?.LastDescendant() ?? this;

    internal virtual ISymbolDeclarationNode Inherited_ContainingDeclaration(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ContainingDeclaration), child, descendant);

    internal virtual IPackageDeclarationNode Inherited_Package(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_Package), child, descendant);

    internal virtual CodeFile Inherited_File(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_File), child, descendant);

    internal virtual PackageNameScope Inherited_PackageNameScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_PackageNameScope), child, descendant);

    internal virtual LexicalScope Inherited_ContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ContainingLexicalScope), child, descendant);

    internal virtual IDeclaredUserType Inherited_ContainingDeclaredType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ContainingDeclaredType), child, descendant);

    internal virtual Pseudotype? Inherited_MethodSelfType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_MethodSelfType), child, descendant);

    internal virtual ITypeDefinitionNode Inherited_ContainingTypeDefinition(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ContainingTypeDefinition), child, descendant);

    internal virtual bool Inherited_IsAttributeType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_IsAttributeType), child, descendant);

    internal virtual ISymbolTree Inherited_SymbolTree(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_SymbolTree), child, descendant);

    internal virtual IPackageFacetDeclarationNode Inherited_Facet(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_Facet), child, descendant);

    internal virtual IFlowState Inherited_FlowStateBefore(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_FlowStateBefore), child, descendant);

    internal virtual IMaybeAntetype Inherited_ContextBindingAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ContextBindingAntetype), child, descendant);

    internal virtual DataType Inherited_ContextBindingType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ContextBindingType), child, descendant);

    internal virtual ValueId? Inherited_MatchReferentValueId(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ContextBindingType), child, descendant);

    internal virtual IMaybeExpressionAntetype? Inherited_ExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ExpectedAntetype), child, descendant);

    internal virtual DataType? Inherited_ExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ExpectedType), child, descendant);

    internal virtual DataType? Inherited_ExpectedReturnType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ExpectedReturnType), child, descendant);

    internal virtual ControlFlowSet Inherited_ControlFlowFollowing(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ControlFlowFollowing), child, descendant);

    internal virtual FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_VariableBindingsMap), child, descendant);

    internal virtual IEntryNode Inherited_ControlFlowEntry(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ControlFlowEntry), child, descendant);

    internal virtual IExitNode Inherited_ControlFlowExit(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ControlFlowExit), child, descendant);

    internal virtual bool Inherited_ImplicitRecoveryAllowed(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ImplicitRecoveryAllowed), child, descendant);

    internal virtual bool Inherited_ShouldPrepareToReturn(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw Child.InheritFailed(nameof(Inherited_ShouldPrepareToReturn), child, descendant);

    internal virtual IPreviousValueId Previous_PreviousValueId(IChildNode before, IInheritanceContext ctx)
        => throw Child.PreviousFailed(nameof(Previous_PreviousValueId), before);

    protected virtual void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        foreach (var child in this.Children().Cast<SemanticNode>())
            child.CollectDiagnostics(diagnostics);
    }

    internal virtual ControlFlowSet CollectControlFlowPrevious(IControlFlowNode target, IInheritanceContext ctx)
    {
        var previous = new Dictionary<IControlFlowNode, ControlFlowKind>();
        CollectControlFlowPrevious(target, previous);
        return ControlFlowSet.Create(previous);
    }

    protected virtual void CollectControlFlowPrevious(IControlFlowNode target, Dictionary<IControlFlowNode, ControlFlowKind> previous)
    {
        foreach (var child in this.Children().Cast<SemanticNode>())
            child.CollectControlFlowPrevious(target, previous);
    }
}
