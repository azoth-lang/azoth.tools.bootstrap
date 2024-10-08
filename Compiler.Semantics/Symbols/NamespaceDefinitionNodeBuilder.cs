using System.Collections.Concurrent;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal class NamespaceDefinitionNodeBuilder
{
    private readonly PackageSymbol packageSymbol;
    private readonly ConcurrentDictionary<NamespaceSymbol, ConcurrentDictionary<IdentifierName, NamespaceSymbol>> childNamespaces = new();
    private readonly MultiMapHashSet<NamespaceSymbol, IFacetMemberDefinitionNode> childMembers = [];

    public NamespaceDefinitionNodeBuilder(PackageSymbol packageSymbol)
    {
        this.packageSymbol = packageSymbol;
        childNamespaces.GetOrAdd(packageSymbol, new ConcurrentDictionary<IdentifierName, NamespaceSymbol>());
        childMembers.Add(packageSymbol, []);
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
                childMembers.Add(current, []);
        }
        // Make childNamespaces contains an entry to the final namespace
        childNamespaces.GetOrAdd(current, _ => new());
        return current;
    }

    public void Add(NamespaceSymbol namespaceSymbol, IFacetMemberDefinitionNode declarationNode)
        => childMembers.TryToAddMapping(namespaceSymbol, declarationNode);

    public INamespaceDefinitionNode Build() => Build(packageSymbol);

    private INamespaceDefinitionNode Build(NamespaceSymbol ns)
    {
        var children = childNamespaces[ns].Values.Select(Build);
        return INamespaceDefinitionNode.Create(ns, children, childMembers[ns]);
    }
}
