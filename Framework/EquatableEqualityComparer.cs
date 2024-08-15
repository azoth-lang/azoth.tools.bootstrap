using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Framework;

public sealed class EquatableEqualityComparer<T> : IEqualityComparer<T?>
    where T : IEquatable<T>?
{
    #region Singleton
    public static readonly EquatableEqualityComparer<T> Instance = new EquatableEqualityComparer<T>();

    private EquatableEqualityComparer() { }
    #endregion

    public bool Equals(T? x, T? y)
    {
        if (x is null) return y is null;
        return x.Equals(y);
    }

    public int GetHashCode([DisallowNull] T value) => value.GetHashCode();
}
