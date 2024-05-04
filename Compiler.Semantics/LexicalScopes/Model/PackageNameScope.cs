using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

public sealed class PackageNameScope
{
    private readonly FixedDictionary<IdentifierName, IFixedSet<IPackageFacetSymbolNode>> facetLookup;
    public PackageSymbol CurrentPackage { get; set; }

    public PackageNameScope(PackageSymbol currentPackage, IEnumerable<IPackageFacetSymbolNode> facets)
    {
        facetLookup = facets.GroupBy(f => f.PackageAliasOrName ?? currentPackage.Name)
                            .ToFixedDictionary(g => g.Key, g => g.ToFixedSet());
        CurrentPackage = currentPackage;
    }

    /// <summary>
    /// Lookup a package by name or <see langword="null"/> for the current package.
    /// </summary>
    /// <remarks>If the current package is looked up by name, no results will be returned. The
    /// current package cannot be referred to by name within the package.</remarks>
    public IEnumerable<INamespaceSymbolNode> LookupPackage(IdentifierName? packageName)
    {
        if (packageName == CurrentPackage.Name)
            throw new InvalidOperationException("Cannot refer to the current package by name.");
        if (facetLookup.TryGetValue(packageName ?? CurrentPackage.Name, out var facets))
            return facets.Select(f => f.GlobalNamespace);
        return Enumerable.Empty<INamespaceSymbolNode>();
    }
}
