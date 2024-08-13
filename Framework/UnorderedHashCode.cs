using System;
using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Framework;

public struct UnorderedHashCode
{
    private int value;
    private int count;

    public void Add<T1>(T1 value1)
    {
        value ^= HashCode.Combine(value1);
        count++;
    }

    public void Add<T1, T2>(T1 value1, T2 value2)
    {
        value ^= HashCode.Combine(value1, value2);
        count++;
    }

    public void Add<T1, T2>(KeyValuePair<T1, T2> pair)
    {
        value ^= HashCode.Combine(pair.Key, pair.Value);
        count++;
    }

    public readonly int ToHashCode() => HashCode.Combine(count, value);

    public readonly override int GetHashCode() => HashCode.Combine(count, value);
}
