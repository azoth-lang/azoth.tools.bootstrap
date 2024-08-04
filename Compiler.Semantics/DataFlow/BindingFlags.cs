using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;

public class BindingFlags<TSymbol>
    where TSymbol : IBindingSymbol
{
    private readonly FixedDictionary<TSymbol, int> symbolMap;
    private readonly BitArray flags;

    internal BindingFlags(
        FixedDictionary<TSymbol, int> symbolMap,
        BitArray flags)
    {
        this.symbolMap = symbolMap;
        this.flags = flags;
    }

    /// <summary>
    /// Returns the state for the symbol.
    /// </summary>
    public bool this[TSymbol symbol]
        => flags[symbolMap[symbol]];

    public BindingFlags<TSymbol> Set(TSymbol symbol, bool value)
    {
        // TODO if setting to the current value, don't need to clone
        var newFlags = Clone();
        newFlags.flags[symbolMap[symbol]] = value;
        return newFlags;
    }

    public BindingFlags<TSymbol> Set(IEnumerable<TSymbol> symbols, bool value)
    {
        // TODO if setting to the current value, don't need to clone
        var newFlags = Clone();
        foreach (var symbol in symbols)
            newFlags.flags[symbolMap[symbol]] = value;

        return newFlags;
    }

    private BindingFlags<TSymbol> Clone() => new(symbolMap, (BitArray)flags.Clone());
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
}
