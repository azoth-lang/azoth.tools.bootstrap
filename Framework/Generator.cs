using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Framework;

/// <summary>
/// Generates an infinite <see cref="IEnumerable{T}"/> by repeatedly calling
/// a generator function.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct Generator<T> : IEnumerable<T>
{
    private readonly Func<T> generator;

    public Generator(Func<T> generator)
    {
        this.generator = generator;
    }

    public IEnumerator<T> GetEnumerator()
    {
        while (true)
            yield return generator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
