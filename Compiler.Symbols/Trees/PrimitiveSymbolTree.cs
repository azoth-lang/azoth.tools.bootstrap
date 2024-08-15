using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

/// <summary>
/// A symbol tree for the definition of all the primitive symbols. This special tree allows for a
/// <see langword="null"/> package.
/// </summary>
public sealed class PrimitiveSymbolTree : ISymbolTree
{
    PackageSymbol? ISymbolTree.Package => null;
    public IFixedSet<Symbol> GlobalSymbols { get; }
    private readonly FixedDictionary<Symbol, IFixedSet<Symbol>> symbolChildren;
    public IEnumerable<Symbol> Symbols => symbolChildren.Keys;
    private readonly FixedDictionary<DeclaredType, PrimitiveTypeSymbol> primitiveSymbolTypeLookup;
    private readonly FixedDictionary<TypeName, TypeSymbol> symbolNameLookup;
    private readonly PrimitiveTypeSymbol anyTypeSymbol;

    public PrimitiveSymbolTree(FixedDictionary<Symbol, IFixedSet<Symbol>> symbolChildren)
    {
        this.symbolChildren = symbolChildren;
        GlobalSymbols = symbolChildren.Keys.Where(s => s.ContainingSymbol is null).ToFixedSet();
        var primitiveSymbols = GlobalSymbols.OfType<PrimitiveTypeSymbol>().ToFixedSet();
        primitiveSymbolTypeLookup = primitiveSymbols.ToFixedDictionary(s => s.DeclaresType);
        anyTypeSymbol = primitiveSymbolTypeLookup[DeclaredType.Any];
        symbolNameLookup = GlobalSymbols.OfType<TypeSymbol>().ToFixedDictionary(s => s.Name!);
    }

    public bool Contains(Symbol symbol) => symbolChildren.ContainsKey(symbol);

    public IEnumerable<Symbol> GetChildrenOf(Symbol symbol)
    {
        if (symbol.Package is not null)
            throw new ArgumentException("Symbol must be primitive", nameof(symbol));

        return symbolChildren.TryGetValue(symbol, out var children)
            ? children : FixedSet.Empty<Symbol>();
    }

    public PrimitiveTypeSymbol LookupSymbolForType(SimpleType type)
        => primitiveSymbolTypeLookup[type];

    public PrimitiveTypeSymbol LookupSymbolForType(AnyType _) => anyTypeSymbol;

    public TypeSymbol LookupSymbol(SpecialTypeName name) => symbolNameLookup[name];
}
