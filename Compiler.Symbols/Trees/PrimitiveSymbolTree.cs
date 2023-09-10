using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

/// <summary>
/// A symbol tree for the definition of all the primitive symbols. This special tree allows for a
/// <see langword="null"/> package.
/// </summary>
public sealed class PrimitiveSymbolTree : ISymbolTree
{
    PackageSymbol? ISymbolTree.Package => null;
    public FixedSet<Symbol> GlobalSymbols { get; }
    private readonly FixedDictionary<Symbol, FixedSet<Symbol>> symbolChildren;
    public IEnumerable<Symbol> Symbols => symbolChildren.Keys;

    public PrimitiveSymbolTree(FixedDictionary<Symbol, FixedSet<Symbol>> symbolChildren)
    {
        this.symbolChildren = symbolChildren;
        GlobalSymbols = symbolChildren.Keys.Where(s => s.ContainingSymbol is null).ToFixedSet();
    }

    public bool Contains(Symbol symbol) => symbolChildren.ContainsKey(symbol);

    public IEnumerable<Symbol> Children(Symbol symbol)
    {
        if (symbol.Package is not null)
            throw new ArgumentException("Symbol must be primitive", nameof(symbol));

        return symbolChildren.TryGetValue(symbol, out var children)
            ? children : FixedSet<Symbol>.Empty;
    }
}
