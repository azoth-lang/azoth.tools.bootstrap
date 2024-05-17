using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

public sealed class PackageNameScope
{
    /// <summary>
    /// The global scope that spans all packages (current and referenced).
    /// </summary>
    /// <remarks>This is the root scope used for using directives.</remarks>
    public NamespaceScope UsingGlobalScope { get; }

    /// <summary>
    /// The global scope for this package.
    /// </summary>
    /// <remarks>This scope first searches the current scope, but then its parent searches the global
    /// scope of all referenced packages.</remarks>
    public NamespaceScope PackageGlobalScope { get; }

    private readonly FixedDictionary<IdentifierName, NamespaceScope> referencedGlobalScopes;

    public PackageNameScope(IEnumerable<IPackageFacetDeclarationNode> packageFacets, IEnumerable<IPackageFacetDeclarationNode> referencedFacets)
    {
        var packageGlobalNamespaces = packageFacets.Select(f => f.GlobalNamespace).ToFixedSet();
        var referencedGlobalNamespaces = referencedFacets.Select(f => f.GlobalNamespace).ToFixedSet();

        UsingGlobalScope = new NamespaceScope(this, packageGlobalNamespaces.Concat(referencedGlobalNamespaces));

        // That parent scope is like the ReferencedGlobalScope, but includes nested namespaces.
        var parent = new NamespaceScope(this, referencedGlobalNamespaces);
        PackageGlobalScope = new NamespaceScope(parent, packageGlobalNamespaces);

        referencedGlobalScopes = referencedGlobalNamespaces.GroupBy(ns => ns.Package.Name)
            .ToFixedDictionary(g => g.Key, g => new NamespaceScope(this, g));
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
