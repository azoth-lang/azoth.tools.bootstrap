using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class DeclarationsAttribute
{
    public static IFixedSet<IPackageMemberDeclarationNode> PackageFacet(IPackageFacetNode node)
    {
        return node.CompilationUnits.SelectMany(n => n.Declarations)
                   .SelectMany(n => MoreEnumerable.TraverseDepthFirst(n, NamespaceChildren))
                   .OfType<IPackageMemberDeclarationNode>().ToFixedSet();

        static IEnumerable<INamespaceMemberDeclarationNode> NamespaceChildren(INamespaceMemberDeclarationNode m)
            => (m as INamespaceDeclarationNode)?.Declarations ?? Enumerable.Empty<INamespaceMemberDeclarationNode>();
    }
}
