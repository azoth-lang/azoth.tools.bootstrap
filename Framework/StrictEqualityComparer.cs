using System;
using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Framework;

/// <summary>
/// An equality comparer that only supports comparing types that implement
/// <see cref="IEquatable{T}"/>.
/// </summary>
/// <remarks>If the type does not implement <see cref="IEquatable{T}"/> then the comparer throws
/// <see cref="NotSupportedException"/>.</remarks>
public static class StrictEqualityComparer<T>
{
    public static readonly IEqualityComparer<T> Instance = Create();

    private static IEqualityComparer<T> Create()
    {
        var type = typeof(T);

        // The types for which the default comparer is not the same as the ReferenceEqualityComparer.
        if (type.IsEquatableToSupertype() || type.IsNullableOfEquatable() || type.IsEnum)
            return EqualityComparer<T>.Default;

        return new Unsupported();
    }

    private class Unsupported : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y)
            => throw new NotSupportedException($"{typeof(T).GetFriendlyName()} does not support strict Equals.");

        public int GetHashCode(T obj)
            => throw new NotSupportedException($"{typeof(T).GetFriendlyName()} does not support strict GetHashCode.");
    }
}
