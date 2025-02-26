using System.Collections.Concurrent;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class MethodSignatureCache
{
    private readonly ConcurrentDictionary<MethodSymbol, MethodSignature> methods = new(ReferenceEqualityComparer.Instance);

    public MethodSignature this[MethodSymbol symbol]
    {
        [Inline(InlineBehavior.Remove)]
        get => methods.GetOrAdd(symbol, Factory);
    }

    private static MethodSignature Factory(MethodSymbol symbol)
        => new(
            symbol.Name,
            symbol.SelfParameterType,
            symbol.ParameterTypes,
            symbol.ReturnType);
}
