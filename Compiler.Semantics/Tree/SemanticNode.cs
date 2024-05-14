using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class SemanticNode : ISemanticNode
{
    public abstract ISyntax? Syntax { get; }

    internal virtual ISymbolNode InheritedContainingSymbolNode(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedContainingSymbolNode), caller, child));

    internal virtual IPackageNode InheritedPackage(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedPackage), caller, child));

    internal virtual CodeFile InheritedFile(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedFile), caller, child));

    internal virtual PackageNameScope InheritedPackageNameScope(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedPackageNameScope), caller, child));

    internal virtual LexicalScope InheritedContainingLexicalScope(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedContainingLexicalScope), caller, child));

    internal virtual IDeclaredUserType InheritedContainingDeclaredType(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedContainingDeclaredType), caller, child));

    internal virtual Pseudotype? InheritedSelfType(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedSelfType), caller, child));

    internal virtual ITypeDeclarationNode InheritedContainingTypeDeclaration(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedContainingTypeDeclaration), caller, child));

    internal virtual bool InheritedIsAttributeType(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedIsAttributeType), caller, child));

    protected virtual void CollectDiagnostics(Diagnostics diagnostics)
    {
        foreach (var child in this.Children().Cast<SemanticNode>())
            child.CollectDiagnostics(diagnostics);
    }
}
