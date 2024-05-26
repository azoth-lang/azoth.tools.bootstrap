using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class SemanticNode : ISemanticNode
{
    public abstract ISyntax? Syntax { get; }

    internal virtual ISymbolDeclarationNode InheritedContainingDeclaration(IChildNode child, IChildNode descendant)
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

    internal virtual LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedContainingLexicalScope), child, descendant));

    internal virtual IDeclaredUserType InheritedContainingDeclaredType(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedContainingDeclaredType), child, descendant));

    internal virtual Pseudotype? InheritedSelfType(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedSelfType), child, descendant));

    internal virtual ITypeDefinitionNode InheritedContainingTypeDeclaration(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedContainingTypeDeclaration), child, descendant));

    internal virtual bool InheritedIsAttributeType(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedIsAttributeType), child, descendant));

    internal virtual ISymbolTree InheritedSymbolTree(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedSymbolTree), child, descendant));

    internal virtual IPackageFacetDeclarationNode InheritedFacet(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedFacet), child, descendant));

    internal virtual ISemanticNode? InheritedPredecessor(IChildNode child, IChildNode descendant)
        => throw new NotImplementedException(Child.InheritFailedMessage(nameof(InheritedPredecessor), child, descendant));

    protected virtual void CollectDiagnostics(Diagnostics diagnostics)
    {
        foreach (var child in this.Children().Cast<SemanticNode>())
            child.CollectDiagnostics(diagnostics);
    }
}
