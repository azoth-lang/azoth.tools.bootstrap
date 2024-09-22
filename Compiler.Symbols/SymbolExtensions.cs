using System;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public static class SymbolExtensions
{
    public static T Assigned<T>(this T? symbol)
        where T : Symbol
        => symbol ?? throw new InvalidOperationException("Symbol not assigned.");
}
