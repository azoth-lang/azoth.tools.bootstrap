using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

public sealed class NamespaceScope : LexicalScope
{
    private readonly LexicalScope? parent;
    public override PackageNameScope PackageNames { get; }
    private readonly IFixedSet<INamespaceSymbolNode> namespaceDeclarations;
    private readonly Dictionary<IdentifierName, NamespaceScope> childScopes = new();

    /// <summary>
    /// Create a top-level namespace scope.
    /// </summary>
    public NamespaceScope(PackageNameScope parent, IEnumerable<INamespaceSymbolNode> namespaceDeclarations)
    {
        PackageNames = parent;
        this.namespaceDeclarations = namespaceDeclarations.ToFixedSet();
    }

    /// <summary>
    /// Create a child namespace scope.
    /// </summary>
    public NamespaceScope(NamespaceScope parent, IEnumerable<INamespaceSymbolNode> namespaceDeclarations)
    {
        this.parent = parent;
        PackageNames = parent.PackageNames;
        this.namespaceDeclarations = namespaceDeclarations.ToFixedSet();
    }

    /// <summary>
    /// Create a child namespace scope that is nested inside a scope with using directives.
    /// </summary>
    public NamespaceScope(UsingDirectivesScope parent, NamespaceScope copyOfScope)
    {
        this.parent = parent;
        PackageNames = parent.PackageNames;
        namespaceDeclarations = copyOfScope.namespaceDeclarations;
    }

    public override NamespaceScope? CreateChildNamespaceScope(IdentifierName namespaceName)
    {
        if (childScopes.TryGetValue(namespaceName, out var childScope)) return childScope;

        var childDeclarations = namespaceDeclarations.SelectMany(d => d.MembersNamed(namespaceName))
                                                     .OfType<INamespaceSymbolNode>().ToFixedSet();
        if (childDeclarations.Count == 0) return null;
        childScope = new NamespaceScope(this, childDeclarations);
        childScopes.Add(namespaceName, childScope);
        return childScope;
    }

    public override IEnumerable<ISymbolNode> Lookup(StandardName name)
    {
        var symbolNodes = namespaceDeclarations
            .SelectMany(ns => ns.MembersNamed(name)).SafeCast<ISymbolNode>()
            .FallbackIfEmpty(namespaceDeclarations.SelectMany(ns => ns.NestedMembersNamed(name)));
        if (parent is not null)
            symbolNodes = symbolNodes.FallbackIfEmpty(() => parent.Lookup(name));
        return symbolNodes;
    }

    public IEnumerable<ISymbolNode> LookupInNamespaceOnly(StandardName name)
        => namespaceDeclarations.SelectMany(ns => ns.MembersNamed(name));
}
