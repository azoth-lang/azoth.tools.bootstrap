using System;

namespace Azoth.Tools.Bootstrap.Framework;

public abstract class Equatable<T> : IEquatable<T>
{
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is T self && Equals(self);
    }

    public abstract bool Equals(T? other);

    public abstract override int GetHashCode();
}
