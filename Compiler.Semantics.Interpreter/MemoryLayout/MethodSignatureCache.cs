using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Microsoft.Extensions.Caching.Memory;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal class MethodSignatureCache
{
    private readonly MemoryCache cache = new(new MemoryCacheOptions());

    public MethodSignature this[MethodSymbol symbol]
        => cache.GetOrCreate(symbol, Factory)!;

    private static MethodSignature Factory(ICacheEntry entry)
    {
        var symbol = (MethodSymbol)entry.Key;
        return new MethodSignature(
            symbol.Name,
            symbol.SelfParameterType,
            symbol.ParameterTypes,
            symbol.ReturnType);
    }
}
