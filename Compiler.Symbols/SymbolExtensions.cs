using System;
using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public static class SymbolExtensions
{
    [DebuggerStepThrough]
    public static T Assigned<T>(this T? symbol)
        where T : Symbol
        => symbol ?? throw new InvalidOperationException("Symbol not assigned.");
}
