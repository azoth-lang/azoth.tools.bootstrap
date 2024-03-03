using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;

public class BindingFlags
{
    private readonly FixedDictionary<BindingSymbol, int> symbolMap;
    private readonly BitArray flags;

    public static BindingFlags ForVariables(
        IExecutableDeclaration declaration,
        ISymbolTree symbolTree,
        bool defaultValue)
    {
        var executableSymbol = declaration.Symbol;
        var symbolMap = symbolTree.GetChildrenOf(executableSymbol).Cast<BindingSymbol>().Enumerate().ToFixedDictionary();
        var flags = new BitArray(symbolMap.Count, defaultValue);
        return new(symbolMap, flags);
    }

    public static BindingFlags ForVariablesAndFields(
        IExecutableDeclaration declaration,
        ISymbolTree symbolTree,
        bool defaultValue)
    {
        var executableSymbol = declaration.Symbol;
        var variables = symbolTree.GetChildrenOf(executableSymbol).Cast<BindingSymbol>();
        var contextTypeSymbol = executableSymbol.ContextTypeSymbol;
        var fields = contextTypeSymbol is not null
            ? symbolTree.GetChildrenOf(contextTypeSymbol).OfType<FieldSymbol>()
            : Enumerable.Empty<FieldSymbol>();
        var symbolMap = variables.Concat(fields).Enumerate().ToFixedDictionary();
        var flags = new BitArray(symbolMap.Count, defaultValue);
        return new(symbolMap, flags);
    }

    private BindingFlags(
        FixedDictionary<BindingSymbol, int> symbolMap,
        BitArray flags)
    {
        this.symbolMap = symbolMap;
        this.flags = flags;
    }

    /// <summary>
    /// Returns the state for the variable or null if the symbol isn't a
    /// variable.
    /// </summary>
    public bool? this[BindingSymbol symbol] => symbolMap.TryGetValue(symbol, out var i) ? flags[i] : null;

    public BindingFlags Set(BindingSymbol symbol, bool value)
    {
        // TODO if setting to the current value, don't need to clone
        var newFlags = Clone();
        newFlags.flags[symbolMap[symbol]] = value;
        return newFlags;
    }

    public BindingFlags Set(IEnumerable<BindingSymbol> symbols, bool value)
    {
        // TODO if setting to the current value, don't need to clone
        var newFlags = Clone();
        foreach (var symbol in symbols)
            newFlags.flags[symbolMap[symbol]] = value;

        return newFlags;
    }

    private BindingFlags Clone() => new(symbolMap, (BitArray)flags.Clone());
}
