using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

public sealed class PackageNameScope
{
    /// <summary>
    /// The global scope that spans all referenced packages (excluding the current package). Does
    /// not search nested namespaces.
    /// </summary>
    /// <remarks>This is the root scope used for using directives.</remarks>
    public NamespaceScope ReferencedGlobalScope { get; }

    /// <summary>
    /// The global scope for this package.
    /// </summary>
    /// <remarks>This scope first searches the current scope, but then its parent searches the global
    /// scope of all referenced packages.</remarks>
    public NamespaceScope GlobalScope { get; }

    private readonly FixedDictionary<IdentifierName, NamespaceScope> referencedGlobalScopes;

    public PackageNameScope(IEnumerable<IPackageFacetSymbolNode> packageFacets, IEnumerable<IPackageFacetSymbolNode> referencedFacets)
    {
        var referencedGlobalNamespaces = referencedFacets.Select(f => f.GlobalNamespace).ToFixedSet();
        ReferencedGlobalScope = new NamespaceScope(this, referencedGlobalNamespaces, includeNested: false);
        // That parent scope is like the ReferencedGlobalScope, but includes nested namespaces.
        var parent = new NamespaceScope(this, referencedGlobalNamespaces, includeNested: true);
        GlobalScope = new NamespaceScope(parent, packageFacets.Select(f => f.GlobalNamespace));

        referencedGlobalScopes = referencedGlobalNamespaces.GroupBy(ns => ns.Package.Name)
            .ToFixedDictionary(g => g.Key, g => new NamespaceScope(this, g, includeNested: false));
    }

    /// <summary>
    /// Get the global scope for a referenced package.
    /// </summary>
    /// <remarks>This provides the root for names that are package qualified.</remarks>
    public NamespaceScope GlobalScopeForReferencedPackage(IdentifierName packageName)
    {
        if (referencedGlobalScopes.TryGetValue(packageName, out var scope))
            return scope;
        throw new InvalidOperationException($"Package '{packageName}' is not referenced.");
    }
}
