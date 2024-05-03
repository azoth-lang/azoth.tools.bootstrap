using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using SymbolDictionary = Azoth.Tools.Bootstrap.Framework.FixedDictionary<Azoth.Tools.Bootstrap.Compiler.Names.TypeName, Azoth.Tools.Bootstrap.Framework.IFixedSet<Azoth.Tools.Bootstrap.Compiler.Core.Promises.IPromise<Azoth.Tools.Bootstrap.Compiler.Symbols.Symbol>>>;

namespace Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

public class NestedSymbolScope : SymbolScope
{
    internal override PackagesSymbolScope ContainingPackagesScope { get; }
    private readonly SymbolScope containingScope;
    private readonly bool isGlobalScope;
    private readonly SymbolDictionary symbolsInScope;
    private readonly SymbolDictionary symbolsInNestedScopes;

    public static NestedSymbolScope Create(
        SymbolScope containingScope,
        SymbolDictionary symbolsInScope,
        SymbolDictionary? symbolsInNestedScopes = null)
    {
        return new NestedSymbolScope(containingScope, false, symbolsInScope,
            symbolsInNestedScopes ?? SymbolDictionary.Empty);
    }

    public static NestedSymbolScope CreateGlobal(
        SymbolScope containingScope,
        SymbolDictionary symbolsInScope,
        SymbolDictionary? symbolsInNestedScopes)
    {
        return new NestedSymbolScope(containingScope, true, symbolsInScope,
            symbolsInNestedScopes ?? SymbolDictionary.Empty);
    }

    private NestedSymbolScope(
        SymbolScope containingScope,
        bool isGlobalScope,
        SymbolDictionary symbolsInScope,
        SymbolDictionary symbolsInNestedScopes)
    {
        ContainingPackagesScope = containingScope.ContainingPackagesScope;
        this.containingScope = containingScope;
        this.isGlobalScope = isGlobalScope;
        this.symbolsInScope = symbolsInScope;
        this.symbolsInNestedScopes = symbolsInNestedScopes;
    }

    public override IEnumerable<IPromise<Symbol>> Lookup(TypeName name, bool includeNested = true)
    {
        if (symbolsInScope.TryGetValue(name, out var symbols)) return symbols;
        if (includeNested && symbolsInNestedScopes.TryGetValue(name, out symbols)) return symbols;
        return containingScope.Lookup(name, includeNested);
    }
}
