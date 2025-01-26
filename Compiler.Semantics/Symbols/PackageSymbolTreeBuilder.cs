using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal sealed class PackageSymbolTreeBuilder
{
    private readonly IPackageFacetNode facet;
    private readonly ISymbolTreeBuilder tree;
    private readonly Dictionary<TypeSymbol, ITypeDefinitionNode> typeDefinitions;
    private Dictionary<ITypeDefinitionNode, bool>? processed;
    private readonly SymbolForest symbolTrees;

    public PackageSymbolTreeBuilder(
        ISymbolTreeBuilder tree,
        IPackageFacetNode facet,
        SymbolForest symbolTrees)
    {
        this.facet = facet;
        this.tree = tree;
        typeDefinitions = new();
        this.symbolTrees = symbolTrees;
    }

    public FixedSymbolTree Build()
    {
        Namespace(facet.GlobalNamespace);
        AddInheritedSymbols();
        return tree.Build();
    }

    private void Namespace(INamespaceDefinitionNode node)
    {
        tree.Add(node.Symbol);
        NamespaceMembers(node.Members);
    }

    private void NamespaceMembers(IEnumerable<INamespaceMemberDefinitionNode> nodes)
        => nodes.ForEach(NameNamespaceMember);

    private void NameNamespaceMember(INamespaceMemberDefinitionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case INamespaceDefinitionNode n:
                Namespace(n);
                break;
            case ITypeDefinitionNode n:
                TypeDefinition(n);
                break;
            case IFunctionDefinitionNode n:
                if (n.Symbol is { } symbol)
                    tree.Add(symbol);
                break;
        }
    }

    private void TypeDefinition(ITypeDefinitionNode n)
    {
        tree.Add(n.Symbol);
        typeDefinitions.Add(n.Symbol, n);
        TypeMembers(n.Members);
    }

    private void TypeMembers(IEnumerable<ITypeMemberDefinitionNode> nodes)
        => nodes.ForEach(TypeMember);

    private void TypeMember(ITypeMemberDefinitionNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case ITypeDefinitionNode n:
                TypeDefinition(n);
                break;
            case ITypeMemberDefinitionNode n:
                if (n.Symbol is { } symbol)
                    tree.Add(symbol);
                break;
        }
    }

    private void AddInheritedSymbols()
    {
        processed = typeDefinitions.Values.ToDictionaryWithValue(_ => false);

        foreach (var typeDefinition in typeDefinitions.Values)
            AddInheritedSymbols(typeDefinition);
    }

    private void AddInheritedSymbols(ITypeDefinitionNode typeDefinition)
    {
        if (processed![typeDefinition]) return;

        var typeSymbol = typeDefinition.Symbol;

        // Set processed early so that if there is a cycle, we don't infinitely recurse
        processed[typeDefinition] = true;

        foreach (var superTypeName in typeDefinition.AllSupertypeNames)
            AddInheritedSymbols(typeSymbol, superTypeName);

        AddInheritedSymbols(typeSymbol, Primitive.Any);
    }

    private void AddInheritedSymbols(OrdinaryTypeSymbol typeSymbol, ITypeNameNode supertypeName)
    {
        var supertypeSymbol = supertypeName.ReferencedDeclaration?.Symbol;
        if (supertypeSymbol is null) return;
        // Make sure super types in the same package have already had their symbols inherited
        if (typeDefinitions.TryGetValue(supertypeSymbol, out var supertypeDefinition))
            AddInheritedSymbols(supertypeDefinition);

        AddInheritedSymbols(typeSymbol, supertypeSymbol);
    }

    private void AddInheritedSymbols(OrdinaryTypeSymbol typeSymbol, TypeSymbol supertypeSymbol)
    {
        var existingMembers = tree.GetChildrenOf(typeSymbol).ToFixedSet();
        foreach (var symbol in symbolTrees.Children(supertypeSymbol))
        {
            if (symbol is ConstructorSymbol) continue;

            // TODO don't inherit private symbols

            // TODO use proper override determination
            if (existingMembers.Any(s => s.Name == symbol.Name)) continue;

            tree.AddInherited(typeSymbol, symbol);
        }
    }
}
