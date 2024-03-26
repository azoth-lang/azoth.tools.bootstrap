using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class NonMemberDeclarationSyntax : DeclarationSyntax, INonMemberDeclarationSyntax
{
    public NamespaceName ContainingNamespaceName { get; }

    private NamespaceOrPackageSymbol? containingNamespaceSymbol;
    public NamespaceOrPackageSymbol ContainingNamespaceSymbol
    {
        get =>
            containingNamespaceSymbol
            ?? throw new InvalidOperationException($"{ContainingNamespaceSymbol} not yet assigned");
        set
        {
            if (containingNamespaceSymbol is not null)
                throw new InvalidOperationException($"Can't set {nameof(ContainingNamespaceSymbol)} repeatedly");
            containingNamespaceSymbol = value;
        }
    }

    protected NonMemberDeclarationSyntax(
        NamespaceName containingNamespaceName,
        TextSpan span,
        CodeFile file,
        TypeName? name,
        TextSpan nameSpan,
        IPromise<Symbol> symbol)
        : base(span, file, name, nameSpan, symbol)
    {
        ContainingNamespaceName = containingNamespaceName;
    }
}
