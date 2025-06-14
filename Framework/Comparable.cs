using System;

namespace Azoth.Tools.Bootstrap.Framework;

public abstract class Comparable<T> : Equatable<T>, IComparable<T>, IComparable
{
    public override bool Equals(T? other) => CompareTo(other) == 0;

    public abstract int CompareTo(T? other);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is T self) return CompareTo(self);
        throw new ArgumentException($"Argument is not of type {typeof(T).GetFriendlyName()}");
    }
}
