using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using ExhaustiveMatching;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal sealed class PackageSymbols : IPackageSymbols
{
    public PackageSymbol PackageSymbol { get; }
    public FixedSymbolTree SymbolTree { get; }
    public FixedSymbolTree TestingSymbolTree { get; }

    public PackageSymbols(PackageSymbol packageSymbol, IPackageFacetNode mainFacet, IPackageFacetNode testingFacet)
    {
        PackageSymbol = packageSymbol;
        SymbolTree = BuildTree(packageSymbol, mainFacet);
        TestingSymbolTree = BuildTree(packageSymbol, testingFacet);
    }

    private static FixedSymbolTree BuildTree(PackageSymbol packageSymbol, IPackageFacetNode facet)
    {
        var tree = new SymbolTreeBuilder(packageSymbol);
        Namespace(facet.GlobalNamespace, tree);
        return tree.Build();
    }

    private static void Namespace(INamespaceDefinitionNode node, ISymbolTreeBuilder tree)
    {
        tree.Add(node.Symbol);
        NamespaceMembers(node.Members, tree);
    }

    private static void NamespaceMembers(IEnumerable<INamespaceMemberDefinitionNode> nodes, ISymbolTreeBuilder tree)
        => nodes.ForEach(n => NameNamespaceMember(n, tree));

    private static void NameNamespaceMember(INamespaceMemberDefinitionNode node, ISymbolTreeBuilder tree)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case INamespaceDefinitionNode n:
                Namespace(n, tree);
                break;
            case ITypeDefinitionNode n:
                TypeDefinition(tree, n);
                break;
            case IFunctionDefinitionNode n:
                tree.Add(n.Symbol);
                break;
        }
    }

    private static void TypeDefinition(ISymbolTreeBuilder tree, ITypeDefinitionNode n)
    {
        tree.Add(n.Symbol);
        TypeMembers(n.Members, tree);
    }

    private static void TypeMembers(IEnumerable<ITypeMemberDefinitionNode> nodes, ISymbolTreeBuilder tree)
        => nodes.ForEach(n => TypeMember(n, tree));

    private static void TypeMember(ITypeMemberDefinitionNode node, ISymbolTreeBuilder tree)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case ITypeDefinitionNode n:
                TypeDefinition(tree, n);
                break;
            case ITypeMemberDefinitionNode n:
                tree.Add(n.Symbol);
                break;
        }
    }
}
