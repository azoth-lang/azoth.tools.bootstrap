using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

/// <summary>
/// A symbol tree for a specific package facet
/// </summary>
public class FixedSymbolTree : ISymbolTree
{
    public PackageSymbol Package => Facet.Package;
    public PackageFacetSymbol Facet { get; }
    private readonly FixedDictionary<Symbol, IFixedSet<Symbol>> symbolChildren;
    public IEnumerable<Symbol> Symbols => symbolChildren.Keys.Prepend(Package);

    public FixedSymbolTree(PackageFacetSymbol facet, FixedDictionary<Symbol, IFixedSet<Symbol>> symbolChildren)
    {
        if (symbolChildren.Keys.Any(s => s.Facet != facet))
            throw new ArgumentException("Children must be for this package", nameof(symbolChildren));
        Facet = facet;
        this.symbolChildren = symbolChildren;
    }

    public bool Contains(Symbol symbol) => symbolChildren.ContainsKey(symbol);

    public IEnumerable<Symbol> GetChildrenOf(Symbol symbol)
    {
        if (symbol.Package != Package)
            throw new ArgumentException("Symbol must be for the package of this tree", nameof(symbol));

        return symbolChildren.TryGetValue(symbol, out var children)
            ? children : FixedSet.Empty<Symbol>();
    }
}
