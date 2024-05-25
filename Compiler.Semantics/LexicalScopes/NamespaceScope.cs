using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public sealed class NamespaceScope : NamespaceSearchScope
{
    private readonly LexicalScope? parent;
    public override PackageNameScope PackageNames { get; }
    private readonly IFixedSet<INamespaceDeclarationNode> namespaceDeclarations;
    private readonly Dictionary<IdentifierName, NamespaceScope> childScopes = new();

    /// <summary>
    /// Create a top-level namespace scope.
    /// </summary>
    internal NamespaceScope(PackageNameScope parent, IEnumerable<INamespaceDeclarationNode> namespaceDeclarations)
    {
        PackageNames = parent;
        this.namespaceDeclarations = namespaceDeclarations.ToFixedSet();
    }

    /// <summary>
    /// Create a child namespace scope.
    /// </summary>
    internal NamespaceScope(NamespaceScope parent, IEnumerable<INamespaceDeclarationNode> namespaceDeclarations)
    {
        this.parent = parent;
        PackageNames = parent.PackageNames;
        this.namespaceDeclarations = namespaceDeclarations.ToFixedSet();
    }

    /// <summary>
    /// Create a child namespace scope that is nested inside a scope with using directives.
    /// </summary>
    internal NamespaceScope(UsingDirectivesScope parent, NamespaceScope copyFromScope)
    {
        this.parent = parent;
        PackageNames = parent.PackageNames;
        namespaceDeclarations = copyFromScope.namespaceDeclarations;
    }

    public override NamespaceScope? CreateChildNamespaceScope(IdentifierName namespaceName)
    {
        if (childScopes.TryGetValue(namespaceName, out var childScope)) return childScope;

        var childDeclarations = namespaceDeclarations.SelectMany(d => d.MembersNamed(namespaceName))
                                                     .OfType<INamespaceDeclarationNode>().ToFixedSet();
        if (childDeclarations.Count == 0) return null;
        childScope = new NamespaceScope(this, childDeclarations);
        childScopes.Add(namespaceName, childScope);
        return childScope;
    }

    public override IEnumerable<IDeclarationNode> Lookup(StandardName name)
    {
        var symbolNodes = namespaceDeclarations
            .SelectMany(ns => ns.MembersNamed(name)).SafeCast<IDeclarationNode>()
            .FallbackIfEmpty(namespaceDeclarations.SelectMany(ns => ns.NestedMembersNamed(name)));
        if (parent is not null)
            symbolNodes = symbolNodes.FallbackIfEmpty(() => parent.Lookup(name));
        return symbolNodes;
    }

    public IEnumerable<IDeclarationNode> LookupInNamespaceOnly(StandardName name)
        => namespaceDeclarations.SelectMany(ns => ns.MembersNamed(name));
}
