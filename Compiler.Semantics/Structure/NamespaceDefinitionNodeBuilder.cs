using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal class NamespaceDefinitionNodeBuilder
{
    private readonly Dictionary<NamespaceName, Dictionary<IdentifierName, NamespaceName>> childNamespaces = new();
    private readonly MultiMapHashSet<NamespaceName, IFacetMemberDefinitionNode> childMembers = [];

    public NamespaceDefinitionNodeBuilder()
    {
        childNamespaces.Add(NamespaceName.Global, new());
        childMembers.Add(NamespaceName.Global, []);
    }

    public NamespaceName AddNamespace(NamespaceName containingNamespace, NamespaceName declaredNamespaceNames)
    {
        var current = containingNamespace;
        foreach (var name in declaredNamespaceNames.Segments)
        {
            var lookup = childNamespaces.GetOrAdd(current, _ => new());
            current = lookup.GetOrAdd(name, n => current.Qualify(n));
            // Make sure childMembers contains and entry to the namespace
            childMembers.TryAdd(current, []);
        }
        // Make sure childNamespaces contains an entry to the final namespace
        childNamespaces.TryAdd(current, _ => new());
        return current;
    }

    public void Add(NamespaceName ns, IFacetMemberDefinitionNode declarationNode)
        => childMembers.TryToAddMapping(ns, declarationNode);

    public INamespaceDefinitionNode Build() => Build(NamespaceName.Global);

    private INamespaceDefinitionNode Build(NamespaceName ns)
    {
        var children = childNamespaces[ns].Values.Select(Build);
        return INamespaceDefinitionNode.Create(ns, children, childMembers[ns]);
    }
}
