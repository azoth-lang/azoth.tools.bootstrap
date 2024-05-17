using System.Collections.Concurrent;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;

internal class SemanticNamespaceSymbolNodeBuilder
{
    private readonly PackageSymbol packageSymbol;
    private readonly ConcurrentDictionary<NamespaceSymbol, ConcurrentDictionary<IdentifierName, NamespaceSymbol>> childNamespaces = new();
    private readonly MultiMapHashSet<NamespaceSymbol, IPackageMemberDefinitionNode> childMembers = new();

    public SemanticNamespaceSymbolNodeBuilder(PackageSymbol packageSymbol)
    {
        this.packageSymbol = packageSymbol;
        childNamespaces.GetOrAdd(packageSymbol, new ConcurrentDictionary<IdentifierName, NamespaceSymbol>());
        childMembers.Add(packageSymbol, new());
    }

    public NamespaceSymbol AddNamespace(NamespaceSymbol containingNamespace, NamespaceName ns)
    {
        var current = containingNamespace;
        foreach (var name in ns.Segments)
        {
            var lookup = childNamespaces.GetOrAdd(current, _ => new());
            current = lookup.GetOrAdd(name, n => new LocalNamespaceSymbol(current, n));
            // Make sure childMembers contains and entry to the namespace
            if (!childMembers.ContainsKey(current))
                childMembers.Add(current, new());
        }
        // Make childNamespaces contains an entry to the final namespace
        childNamespaces.GetOrAdd(current, _ => new());
        return current;
    }

    public void Add(NamespaceSymbol namespaceSymbol, IPackageMemberDefinitionNode declarationNode)
        => childMembers.TryToAddMapping(namespaceSymbol, declarationNode);

    public INamespaceDefinitionNode Build() => Build(packageSymbol);

    private INamespaceDefinitionNode Build(NamespaceSymbol ns)
    {
        var children = childNamespaces[ns].Values.Select(Build);
        return new NamespaceDefinitionNode(ns, children, childMembers[ns]);
    }
}
