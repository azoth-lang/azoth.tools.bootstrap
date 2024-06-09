using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Framework;

public static class MultiMapExtensions
{
    public static MultiMapHashSet<TKey, TValue> ToMultiMapHashSet<TKey, TValue>(this IEnumerable<TValue> source,
        Func<TValue, TKey> keySelector)
        where TKey : notnull
        => new(source.GroupBy(keySelector).ToDictionary(g => g.Key, g => g.ToHashSet()));
}
