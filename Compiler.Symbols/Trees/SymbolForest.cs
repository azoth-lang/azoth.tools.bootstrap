using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

public class SymbolForest
{
    public PrimitiveSymbolTree PrimitiveSymbolTree { get; }
    private readonly FixedDictionary<PackageFacetSymbol, ISymbolTree> facetTrees;
    public IEnumerable<PackageFacetSymbol> PackageFacets => facetTrees.Keys;

    public IEnumerable<Symbol> GlobalSymbols => PrimitiveSymbolTree.GlobalSymbols
                                                                   .Concat(PackageFacets.SelectMany(Children));
    public IEnumerable<Symbol> Symbols => facetTrees.Values.SelectMany(t => t.Symbols);

    public SymbolForest(PrimitiveSymbolTree primitiveSymbolTree, FixedSymbolTree intrinsicSymbolTree,
        ISymbolTreeBuilder symbolTreeBuilder, IEnumerable<FixedSymbolTree> otherTrees)
    {
        if (symbolTreeBuilder.Package is null)
            throw new ArgumentException("Can't be builder for primitive symbols", nameof(symbolTreeBuilder));
        PrimitiveSymbolTree = primitiveSymbolTree;
        facetTrees = otherTrees.Append<ISymbolTree>(symbolTreeBuilder).Append(intrinsicSymbolTree)
                               .ToFixedDictionary(t => t.Facet!);
    }

    public SymbolForest(PrimitiveSymbolTree primitiveSymbolTree, FixedSymbolTree intrinsicSymbolTree, IEnumerable<FixedSymbolTree> otherTrees)
    {
        PrimitiveSymbolTree = primitiveSymbolTree;
        facetTrees = otherTrees.Append(intrinsicSymbolTree).SafeCast<ISymbolTree>()
                               .ToFixedDictionary(t => t.Facet!);
    }

    public IEnumerable<Symbol> Children(Symbol symbol)
    {
        if (symbol.Facet is null)
            return PrimitiveSymbolTree.GetChildrenOf(symbol);

        if (!facetTrees.TryGetValue(symbol.Facet, out var tree))
            throw new ArgumentException("Symbol must be for one of the package facets in this forest.", nameof(symbol));

        return tree.GetChildrenOf(symbol);
    }
}
