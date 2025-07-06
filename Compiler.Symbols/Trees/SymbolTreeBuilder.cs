using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

/// <summary>
/// Builder for a <see cref="ISymbolTree"/>
/// </summary>
public class SymbolTreeBuilder : ISymbolTreeBuilder
{
    public static SymbolTreeBuilder CreateForPrimitives() => new();

    public PackageSymbol? Package => Facet?.Package;
    public PackageFacetSymbol? Facet { get; }
    private readonly Dictionary<Symbol, ISet<Symbol>> symbolChildren = new Dictionary<Symbol, ISet<Symbol>>();
    public IEnumerable<Symbol> Symbols => symbolChildren.Keys;

    private SymbolTreeBuilder()
    {
        Facet = null;
    }

    public SymbolTreeBuilder(PackageFacetSymbol facet)
    {
        Facet = facet;
        symbolChildren.Add(facet, new HashSet<Symbol>());
    }

    public bool Contains(Symbol symbol)
        => symbolChildren.ContainsKey(symbol);

    public IEnumerable<Symbol> GetChildrenOf(Symbol symbol)
    {
        RequireForPackage(symbol);
        return symbolChildren.TryGetValue(symbol, out var children)
            ? children : [];
    }

    public void Add(Symbol symbol)
    {
        RequireForPackage(symbol);
        GetOrAdd(symbol);
    }

    public void AddInherited(TypeSymbol symbol, Symbol inheritedSymbol)
    {
        RequireForPackage(symbol);
        GetOrAdd(symbol).Add(inheritedSymbol);
    }

    private void RequireForPackage(Symbol symbol)
    {
        if (symbol.Facet != Facet)
            throw new ArgumentException("Symbol must be for the package of this tree", nameof(symbol));
    }

    private ISet<Symbol> GetOrAdd(Symbol symbol)
    {
        if (symbolChildren.TryGetValue(symbol, out var children))
            return children;

        // Add to parent's children
        if (symbol.ContainingSymbol is not null)
            GetOrAdd(symbol.ContainingSymbol).Add(symbol);
        children = new HashSet<Symbol>();
        symbolChildren.Add(symbol, children);
        return children;
    }

    public FixedSymbolTree Build()
    {
        if (Facet is null)
            throw new InvalidOperationException($"Can't build {nameof(FixedSymbolTree)} without a package facet.");
        return new(Facet, symbolChildren.ToFixedDictionary(e => e.Key, e => e.Value.ToFixedSet()));
    }

    public PrimitiveSymbolTree BuildPrimitives()
    {
        if (Facet is not null)
            throw new InvalidOperationException($"Can't build {nameof(PrimitiveSymbolTree)} WITH a package facet.");
        return new(symbolChildren.ToFixedDictionary(e => e.Key, e => e.Value.ToFixedSet()));
    }
}
