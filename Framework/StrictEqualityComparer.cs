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
    public static readonly IEqualityComparer<T> Instance = StrictEqualityComparer.Create<T>();
}

internal static class StrictEqualityComparer
{
    public static IEqualityComparer<T> Create<T>()
    {
        var type = typeof(T);

        // The types for which the default comparer is not the same as the ReferenceEqualityComparer.
        if (type.IsEquatableToSupertype()
            || type.IsNullableOfEquatable()
            || type.IsEnum)
            return EqualityComparer<T>.Default;

        return new UnsupportedEqualityComparer<T>();
    }
}
