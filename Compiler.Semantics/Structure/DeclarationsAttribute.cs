using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class DeclarationsAttribute
{
    public static IFixedSet<IPackageMemberDefinitionNode> PackageFacet(IPackageFacetNode node)
    {
        return node.CompilationUnits.SelectMany(n => n.Definitions)
                   .SelectMany(n => MoreEnumerable.TraverseDepthFirst(n, NamespaceChildren))
                   .OfType<IPackageMemberDefinitionNode>().ToFixedSet();

        static IEnumerable<INamespaceMemberDefinitionNode> NamespaceChildren(INamespaceMemberDefinitionNode m)
            => (m as INamespaceDefinitionNode)?.Definitions ?? Enumerable.Empty<INamespaceMemberDefinitionNode>();
    }
}
