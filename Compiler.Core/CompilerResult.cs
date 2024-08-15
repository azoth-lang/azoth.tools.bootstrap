using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

/// <summary>
/// A result that may carry a collection of diagnostics.
/// </summary>
/// <remarks>The presence of diagnostics generally doesn't mean the result cannot be used. Rather
/// that should be indicated with a special value like <see langword="null"/> or the unknown type.</remarks>
public readonly struct CompilerResult<T>
{
    public T Value { get; }
    public IFixedSet<Diagnostic> Diagnostics { get; }

    public CompilerResult(T value, IEnumerable<Diagnostic> diagnostics)
    {
        Value = value;
        Diagnostics = diagnostics.ToFixedSet();
    }

    public CompilerResult<TResult> Select<TResult>(Func<T, TResult> selector)
        => new(selector(Value), Diagnostics);
}
