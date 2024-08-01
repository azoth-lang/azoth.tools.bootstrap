using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class DeclarationsAspect
{
    public static IFixedSet<IPackageMemberDefinitionNode> PackageFacet_Definitions(IPackageFacetNode node)
    {
        return node.CompilationUnits.SelectMany(n => n.Definitions)
                   .SelectMany(n => MoreEnumerable.TraverseDepthFirst(n, NamespaceChildren))
                   .OfType<IPackageMemberDefinitionNode>().ToFixedSet();

        static IEnumerable<INamespaceBlockMemberDefinitionNode> NamespaceChildren(INamespaceBlockMemberDefinitionNode m)
            => (m as INamespaceBlockDefinitionNode)?.Members ?? Enumerable.Empty<INamespaceBlockMemberDefinitionNode>();
    }

    public static IFunctionDefinitionNode? Package_EntryPoint(IPackageNode node)
        // TODO warn on and remove main functions that don't have correct parameters or types
        // TODO compiler error on multiple main functions
        => node.MainFacet.Definitions.OfType<IFunctionDefinitionNode>().SingleOrDefault(f => f.Name == "main");
}
