using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using SymbolDictionary = Azoth.Tools.Bootstrap.Framework.FixedDictionary<Azoth.Tools.Bootstrap.Compiler.Names.TypeName, Azoth.Tools.Bootstrap.Framework.IFixedSet<Azoth.Tools.Bootstrap.Compiler.Core.Promises.IPromise<Azoth.Tools.Bootstrap.Compiler.Symbols.Symbol>>>;

namespace Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

public class NestedScope : LexicalScope
{
    internal override PackagesScope ContainingPackagesScope { get; }
    private readonly LexicalScope containingScope;
    private readonly bool isGlobalScope;
    private readonly SymbolDictionary symbolsInScope;
    private readonly SymbolDictionary symbolsInNestedScopes;

    public static NestedScope Create(
        LexicalScope containingScope,
        SymbolDictionary symbolsInScope,
        SymbolDictionary? symbolsInNestedScopes = null)
    {
        return new NestedScope(containingScope, false, symbolsInScope,
            symbolsInNestedScopes ?? SymbolDictionary.Empty);
    }

    public static NestedScope CreateGlobal(
        LexicalScope containingScope,
        SymbolDictionary symbolsInScope,
        SymbolDictionary? symbolsInNestedScopes)
    {
        return new NestedScope(containingScope, true, symbolsInScope,
            symbolsInNestedScopes ?? SymbolDictionary.Empty);
    }

    private NestedScope(
        LexicalScope containingScope,
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

    public override IEnumerable<IPromise<Symbol>> LookupInGlobalScope(TypeName name)
        => !isGlobalScope ? containingScope.LookupInGlobalScope(name) : Lookup(name, false);

    public override IEnumerable<IPromise<Symbol>> Lookup(TypeName name, bool includeNested = true)
    {
        if (symbolsInScope.TryGetValue(name, out var symbols)) return symbols;
        if (includeNested && symbolsInNestedScopes.TryGetValue(name, out symbols)) return symbols;
        return containingScope.Lookup(name, includeNested);
    }
}
