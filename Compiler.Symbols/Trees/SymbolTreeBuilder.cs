using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

/// <summary>
/// Builder for a <see cref="ISymbolTree"/>
/// </summary>
public class SymbolTreeBuilder : ISymbolTreeBuilder
{
    public static SymbolTreeBuilder CreateForPrimitives() => new();

    public PackageSymbol? Package { get; }
    private readonly IDictionary<Symbol, ISet<Symbol>> symbolChildren = new Dictionary<Symbol, ISet<Symbol>>();
    public IEnumerable<Symbol> Symbols => symbolChildren.Keys;

    private SymbolTreeBuilder()
    {
        Package = null;
    }

    public SymbolTreeBuilder(PackageSymbol package)
    {
        Package = package;
        symbolChildren.Add(package, new HashSet<Symbol>());
    }

    public bool Contains(Symbol symbol)
        => symbolChildren.ContainsKey(symbol);

    public IEnumerable<Symbol> Children(Symbol symbol)
    {
        RequireForPackage(symbol);
        return symbolChildren.TryGetValue(symbol, out var children)
            ? children : Enumerable.Empty<Symbol>();
    }

    public void Add(Symbol symbol)
    {
        RequireForPackage(symbol);
        GetOrAdd(symbol);
    }

    public void Add(IEnumerable<Symbol> symbols)
    {
        foreach (var symbol in symbols)
            Add(symbol);
    }

    public void AddInherited(TypeSymbol symbol, Symbol inheritedSymbol)
    {
        RequireForPackage(symbol);
        GetOrAdd(symbol).Add(inheritedSymbol);
    }

    private void RequireForPackage(Symbol symbol)
    {
        if (symbol.Package != Package)
            throw new ArgumentException("Symbol must be for the package of this tree", nameof(symbol));
    }

    private ISet<Symbol> GetOrAdd(Symbol symbol)
    {
        if (!symbolChildren.TryGetValue(symbol, out var children))
        {
            // Add to parent's children
            if (symbol.ContainingSymbol is not null)
                GetOrAdd(symbol.ContainingSymbol).Add(symbol);
            children = new HashSet<Symbol>();
            symbolChildren.Add(symbol, children);
        }
        return children;
    }

    public FixedSymbolTree Build()
    {
        if (Package is null)
            throw new InvalidOperationException($"Can't build {nameof(FixedSymbolTree)} without a package");
        return new(Package, symbolChildren.ToFixedDictionary(e => e.Key, e => e.Value.ToFixedSet()));
    }

    public PrimitiveSymbolTree BuildPrimitives()
    {
        if (Package is not null)
            throw new InvalidOperationException($"Can't build {nameof(PrimitiveSymbolTree)} WITH a package");
        return new(symbolChildren.ToFixedDictionary(e => e.Key, e => e.Value.ToFixedSet()));
    }
}
