using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;

public sealed class BindingFlags<T> : IEquatable<BindingFlags<T>>
    where T : notnull
{
    private readonly FixedDictionary<T, int> symbolMap;
    private readonly BitArray flags;

    internal BindingFlags(
        FixedDictionary<T, int> symbolMap,
        BitArray flags)
    {
        this.symbolMap = symbolMap;
        this.flags = flags;
    }

    /// <summary>
    /// Returns the state for the symbol.
    /// </summary>
    public bool this[T symbol]
        => flags[symbolMap[symbol]];

    public BindingFlags<T> Set(T symbol, bool value)
    {
        // TODO if setting to the current value, don't need to clone
        var newFlags = Clone();
        newFlags.flags[symbolMap[symbol]] = value;
        return newFlags;
    }

    public BindingFlags<T> Set(IEnumerable<T> symbols, bool value)
    {
        // TODO if setting to the current value, don't need to clone
        var newFlags = Clone();
        foreach (var symbol in symbols)
            newFlags.flags[symbolMap[symbol]] = value;

        return newFlags;
    }

    public BindingFlags<T> Intersect(BindingFlags<T> other)
    {
        Requires.That(nameof(other), ReferenceEquals(symbolMap, other.symbolMap), "Must have same symbol map");
        var newFlags = Clone();
        newFlags.flags.And(other.flags);
        return newFlags;
    }

    private BindingFlags<T> Clone() => new(symbolMap, (BitArray)flags.Clone());

    #region Equality
    public bool Equals(BindingFlags<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        // symbolMap is compared by reference for efficiency because all BindingFlags<T> instances
        // in a given context are supposed to share the same symbol map instance.
        return ReferenceEquals(symbolMap, other.symbolMap) && flags.ValuesEqual(other.flags);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is BindingFlags<T> other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(RuntimeHelpers.GetHashCode(symbolMap), flags.GetValueHashCode());
    #endregion
}

public static class BindingFlags
{
    public static BindingFlags<IVariableSymbol> ForVariables(
        IExecutableDeclaration declaration,
        ISymbolTree symbolTree,
        bool defaultValue)
    {
        var executableSymbol = declaration.Symbol;
        var symbolMap = symbolTree.GetChildrenOf(executableSymbol).Cast<IVariableSymbol>().Enumerate()
                                  .ToFixedDictionary();
        var flags = new BitArray(symbolMap.Count, defaultValue);
        return new(symbolMap, flags);
    }

    public static BindingFlags<IBindingSymbol> ForVariablesAndFields(
        IExecutableDeclaration declaration,
        ISymbolTree symbolTree,
        bool defaultValue)
    {
        var executableSymbol = declaration.Symbol;
        var variables = symbolTree.GetChildrenOf(executableSymbol).Cast<IBindingSymbol>();
        var contextTypeSymbol = executableSymbol.ContextTypeSymbol;
        var fields = contextTypeSymbol is not null
            ? symbolTree.GetChildrenOf(contextTypeSymbol).OfType<FieldSymbol>()
            : [];
        var symbolMap = variables.Concat(fields).Enumerate().ToFixedDictionary();
        var flags = new BitArray(symbolMap.Count, defaultValue);
        return new(symbolMap, flags);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks>This assumes the map is to unique indexes in the range <c>0</c> to <c>Count - 1</c>.</remarks>
    public static BindingFlags<T> Create<T>(FixedDictionary<T, int> map, bool defaultValue)
        where T : notnull
    {
        var flags = new BitArray(map.Count, defaultValue);
        return new(map, flags);
    }
}
