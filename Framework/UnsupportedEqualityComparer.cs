using System;
using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Framework;
internal class UnsupportedEqualityComparer<T> : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y)
        => throw new NotSupportedException($"{typeof(T).GetFriendlyName()} does not support strict Equals.");

    public int GetHashCode(T obj) =>
        throw new NotSupportedException($"{typeof(T).GetFriendlyName()} does not support strict GetHashCode.");
}
