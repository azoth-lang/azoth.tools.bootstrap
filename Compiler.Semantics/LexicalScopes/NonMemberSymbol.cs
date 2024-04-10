using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

/// <summary>
/// A special type for symbols that represent things that are not members of types.
/// </summary>
internal class NonMemberSymbol
{
    public static NonMemberSymbol For(INonMemberEntityDeclarationSyntax declaration)
        => new NonMemberSymbol(declaration);

    public static NonMemberSymbol For(IInitializerDeclarationSyntax declaration)
        => new NonMemberSymbol(declaration);

    public static NonMemberSymbol ForExternalSymbol(Symbol symbol)
        => new NonMemberSymbol(symbol);

    public static NonMemberSymbol ForPackageNamespace(LocalNamespaceSymbol ns)
        => new NonMemberSymbol(ns);

    public bool InCurrentPackage { get; }
    /// <summary>
    /// The namespace this symbol is declared in.
    /// </summary>
    public NamespaceName ContainingNamespace { get; }
    public TypeName Name { get; }
    /// <summary>
    /// The namespace that must exist for this symbol.
    /// </summary>
    /// <remarks>For most symbols, this is just the <see cref="ContainingNamespace"/>, but for
    /// namespace symbols, it is the namespace being declared.</remarks>
    public NamespaceName RequiredNamespace { get; }
    public IPromise<Symbol> Symbol { get; }

    private NonMemberSymbol(INonMemberEntityDeclarationSyntax declaration)
    {
        InCurrentPackage = true;
        ContainingNamespace = declaration.ContainingNamespaceName;
        Name = declaration.Name;
        RequiredNamespace = ContainingNamespace;
        Symbol = declaration.Symbol;
    }

    private NonMemberSymbol(IInitializerDeclarationSyntax declaration)
    {
        if (declaration.Name is not null)
            throw new ArgumentException("Must be for an unnamed initializer");
        InCurrentPackage = true;
        ContainingNamespace = declaration.DeclaringType.ContainingNamespaceName;
        Name = declaration.DeclaringType.Name.Text;
        RequiredNamespace = ContainingNamespace;
        Symbol = declaration.Symbol;
    }

    private NonMemberSymbol(Symbol symbol)
    {
        if (symbol.ContainingSymbol is not (NamespaceOrPackageSymbol or null))
            throw new ArgumentException("Symbol must be for a non-member declaration", nameof(symbol));
        var containingSymbol = symbol.ContainingSymbol as NamespaceOrPackageSymbol;
        InCurrentPackage = false;
        ContainingNamespace = containingSymbol?.NamespaceName ?? NamespaceName.Global;
        Name = symbol.Name ?? throw new ArgumentException("Symbol must have a name", nameof(symbol));
        RequiredNamespace = ContainingNamespace;
        Symbol = Promise.ForValue(symbol);
    }

    private NonMemberSymbol(LocalNamespaceSymbol ns)
    {
        InCurrentPackage = true;
        ContainingNamespace = ns.ContainingSymbol.NamespaceName;
        Name = ns.Name;
        RequiredNamespace = ns.NamespaceName;
        Symbol = Promise.ForValue(ns);
    }
}
