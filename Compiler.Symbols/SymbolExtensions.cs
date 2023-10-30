using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public static class SymbolExtensions
{
    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static TSymbol Assigned<TSymbol>([NotNull] this TSymbol? symbol)
        where TSymbol : Symbol
        => symbol ?? throw new InvalidOperationException("Symbol not assigned.");
}
