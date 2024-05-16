using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;

internal static class NamespaceSymbolCollector
{
    public static void Collect(
        IPackageNode node,
        ISymbolTreeBuilder symbolTree,
        ISymbolTreeBuilder testingSymbolTree)
    {
        PackageFacet(node.MainFacet.SymbolNode, symbolTree);
        PackageFacet(node.TestingFacet.SymbolNode, testingSymbolTree);
    }

    private static void PackageFacet(IPackageFacetDeclarationNode node, ISymbolTreeBuilder symbolTree)
        => Namespace(node.GlobalNamespace, symbolTree);

    private static void Namespace(INamespaceDeclarationNode node, ISymbolTreeBuilder symbolTree)
    {
        symbolTree.Add(node.Symbol);
        Namespaces(node.Members.OfType<INamespaceDeclarationNode>(), symbolTree);
    }

    private static void Namespaces(IEnumerable<INamespaceDeclarationNode> nodes, ISymbolTreeBuilder symbolTree)
        => nodes.ForEach(n => Namespace(n, symbolTree));
}
