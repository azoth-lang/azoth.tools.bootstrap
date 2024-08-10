using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
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

    protected virtual ITreeNode? PeekParent() => null;
    ITreeNode? ITreeNode.PeekParent() => PeekParent();

    internal SemanticNode LastDescendant()
        => ((SemanticNode?)this.Children().LastOrDefault())?.LastDescendant() ?? this;

    internal virtual ISymbolDeclarationNode InheritedContainingDeclaration(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedContainingDeclaration), child, descendant));

    internal virtual IPackageDeclarationNode InheritedPackage(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedPackage), child, descendant));

    internal virtual CodeFile InheritedFile(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedFile), child, descendant));

    internal virtual PackageNameScope InheritedPackageNameScope(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedPackageNameScope), child, descendant));

    internal virtual LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedContainingLexicalScope), child, descendant));

    internal virtual IDeclaredUserType InheritedContainingDeclaredType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedContainingDeclaredType), child, descendant));

    internal virtual Pseudotype? InheritedSelfType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedSelfType), child, descendant));

    internal virtual ITypeDefinitionNode InheritedContainingTypeDefinition(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedContainingTypeDefinition), child, descendant));

    internal virtual bool InheritedIsAttributeType(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedIsAttributeType), child, descendant));

    internal virtual ISymbolTree InheritedSymbolTree(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedSymbolTree), child, descendant));

    internal virtual IPackageFacetDeclarationNode InheritedFacet(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedFacet), child, descendant));

    internal virtual IFlowState InheritedFlowStateBefore(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedFlowStateBefore), child, descendant));

    internal virtual IMaybeAntetype InheritedBindingAntetype(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedBindingAntetype), child, descendant));

    internal virtual DataType InheritedBindingType(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedBindingType), child, descendant));

    internal virtual ValueId? InheritedMatchReferentValueId(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedBindingType), child, descendant));

    internal virtual IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedExpectedAntetype), child, descendant));

    internal virtual DataType? InheritedExpectedReturnType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedExpectedReturnType), child, descendant));

    internal virtual ControlFlowSet InheritedControlFlowFollowing(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedControlFlowFollowing), child, descendant));

    internal virtual FixedDictionary<IVariableBindingNode, int> InheritedVariableBindingsMap(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedVariableBindingsMap), child, descendant));

    internal virtual IEntryNode InheritedControlFlowEntry(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedControlFlowEntry), child, descendant));

    internal virtual IExitNode InheritedControlFlowExit(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedControlFlowExit), child, descendant));

    internal virtual IPreviousValueId PreviousValueId(IChildNode before, IInheritanceContext ctx)
        => throw Child.PreviousFailed(nameof(PreviousValueId), before);

    protected virtual void CollectDiagnostics(DiagnosticsBuilder diagnostics)
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
