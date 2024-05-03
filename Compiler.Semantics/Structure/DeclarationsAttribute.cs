using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class DeclarationsAttribute
{
    public static IFixedSet<IPackageMemberDeclarationNode> PackageDeclarations(PackageNode node)
        => Declarations(node.CompilationUnits);

    public static IFixedSet<IPackageMemberDeclarationNode> PackageTestingDeclarations(PackageNode node)
        => Declarations(node.TestingCompilationUnits);

    private static IFixedSet<IPackageMemberDeclarationNode> Declarations(IEnumerable<ICompilationUnitNode> nodes)
    {
        return nodes.SelectMany(n => n.Declarations)
                    .SelectMany(n => MoreEnumerable.TraverseDepthFirst(n, NamespaceChildren))
                    .OfType<IPackageMemberDeclarationNode>().ToFixedSet();

        static IEnumerable<INamespaceMemberDeclarationNode> NamespaceChildren(INamespaceMemberDeclarationNode m)
            => (m as INamespaceDeclarationNode)?.Declarations ?? Enumerable.Empty<INamespaceMemberDeclarationNode>();
    }
}
