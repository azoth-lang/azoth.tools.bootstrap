using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Framework;

public interface IEqualityComparable<in T>
{
    static abstract IEqualityComparer<T?> EqualityComparer { get; }
}
