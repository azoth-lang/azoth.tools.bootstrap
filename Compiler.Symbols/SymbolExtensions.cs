using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public static class SymbolExtensions
{
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Assigned<T>(this T? symbol)
        where T : Symbol
        => symbol ?? throw new InvalidOperationException("Symbol not assigned.");
}
