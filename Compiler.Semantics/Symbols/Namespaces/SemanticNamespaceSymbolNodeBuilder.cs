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
    private readonly MultiMapHashSet<NamespaceSymbol, ITypeOrFunctionSymbolNode> childMembers = new();

    public SemanticNamespaceSymbolNodeBuilder(PackageSymbol packageSymbol)
    {
        this.packageSymbol = packageSymbol;
        childNamespaces.GetOrAdd(packageSymbol, new ConcurrentDictionary<IdentifierName, NamespaceSymbol>());
        childMembers.Add(packageSymbol, new());
    }

    public NamespaceSymbol AddNamespace(NamespaceSymbol containingNamespace, NamespaceName ns)
    {
        var parent = containingNamespace;
        foreach (var name in ns.Segments)
        {
            var lookup = childNamespaces.GetOrAdd(parent, _ => new());
            parent = lookup.GetOrAdd(name, n => new LocalNamespaceSymbol(parent, n));
            // Make sure childMembers contains and entry to the namespace
            if (!childMembers.ContainsKey(parent))
                childMembers.Add(parent, new());
        }
        // Make childNamespaces contains an entry to the final namespace
        childNamespaces.GetOrAdd(parent, _ => new());
        return parent;
    }

    public void Add(NamespaceSymbol namespaceSymbol, ITypeOrFunctionSymbolNode symbolNode)
        => childMembers.TryToAddMapping(namespaceSymbol, symbolNode);

    public INamespaceSymbolNode Build() => Build(packageSymbol);

    private INamespaceSymbolNode Build(NamespaceSymbol ns)
    {
        var children = childNamespaces[ns].Values.Select(Build);
        return new SemanticNamespaceSymbolNode(ns, children.Concat<INamespaceMemberSymbolNode>(childMembers[ns]));
    }
}
