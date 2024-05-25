using System;

namespace Azoth.Tools.Bootstrap.Framework;

/// <summary>
/// A buffer holding values. Similar to an array, but always uses <see langword="ref"/> access.
/// </summary>
public readonly struct Buffer<T>
{
    public static readonly Buffer<T> Empty = default;

    private readonly T[]? values;

    public Buffer(int count)
        => values = count == 0 ? null : new T[count];

    public Buffer(T[]? values)
        => this.values = values;

    public int Count => values?.Length ?? 0;

    public ref T this[int index]
    {
        get
        {
            var nonEmptyValues = values ?? throw new ArgumentOutOfRangeException(nameof(index));
            return ref nonEmptyValues[index];
        }
    }
}
