using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

/// <summary>
/// The root lexical scope that manages package references and the global scope.
/// </summary>
public sealed class PackageReferenceScope : DeclarationLexicalScope
{
    public override PackageReferenceScope RootScope => this;

    /// <summary>
    /// The package that this scope is a part of.
    /// </summary>
    public PackageSymbol Package { get; }

    private readonly FixedDictionary<IdentifierName, FixedSymbolTree> packageAliases;
    private readonly FixedDictionary<StandardName, IFixedSet<Symbol>> globalNamespace;
    private readonly FixedDictionary<StandardName, IFixedSet<Symbol>> allNamespaces;

    public PackageReferenceScope(
        PackageSymbol package,
        FixedDictionary<IdentifierName, FixedSymbolTree> packageAliases)
    {
        Package = package;
        this.packageAliases = packageAliases;
        var trees = packageAliases.Values
                                  .Append(Intrinsic.SymbolTree)
                                  .ToFixedSet();

        // TODO make computing these lazy?
        // TODO filter to published symbols
        globalNamespace = trees.SelectMany(t => t.GetChildrenOf(t.Package))
                               .Where(s => s.Name is StandardName)
                               .GroupBy(s => (StandardName)s.Name!, Functions.Identity)
                               .ToFixedDictionary(g => g.Key, g => g.ToFixedSet());

        allNamespaces = trees.SelectMany(t => t.Symbols)
                             .Where(s => s is { Name: StandardName, ContainingSymbol: NamespaceSymbol or TypeSymbol })
                             .GroupBy(s => (StandardName)s.Name!, Functions.Identity)
                             .ToFixedDictionary(g => g.Key, g => g.ToFixedSet());
    }

    /// <summary>
    /// Get the symbol tree for the referenced package with the given alias.
    /// </summary>
    public FixedSymbolTree? ReferencedPackage(IdentifierName name)
        => packageAliases.TryGetValue(name, out var package) ? package : null;

    public override IEnumerable<Symbol> ResolveInReferences(StandardName name)
    {
        if (globalNamespace.TryGetValue(name, out var symbols))
            return symbols;
        if (allNamespaces.TryGetValue(name, out symbols))
            return symbols;
        return Enumerable.Empty<Symbol>();
    }
}
