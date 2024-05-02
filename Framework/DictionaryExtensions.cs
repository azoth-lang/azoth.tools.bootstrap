using System;
using System.Collections.Generic;
using System.Linq;
using static Azoth.Tools.Bootstrap.Framework.Functions;

namespace Azoth.Tools.Bootstrap.Framework;

public static class DictionaryExtensions
{
    public static FixedDictionary<TKey, TValue> ToFixedDictionary<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
        => new(dictionary);

    public static FixedDictionary<TKey, TValue> ToFixedDictionary<TSource, TKey, TValue>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector)
        where TKey : notnull
        => new(source.ToDictionary(keySelector, valueSelector));

    public static FixedDictionary<TKey, TValue> ToFixedDictionary<TKey, TValue>(
        this IEnumerable<(TKey Key, TValue Value)> source)
        where TKey : notnull
        => new(source.ToDictionary(s => s.Key, s => s.Value));

    public static FixedDictionary<TKey, TSource> ToFixedDictionary<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
        where TKey : notnull
        => new(source.ToDictionary(keySelector));

    public static Dictionary<TSource, TValue> ToDictionaryWithValue<TSource, TValue>(
        this IEnumerable<TSource> source,
        Func<TSource, TValue> valueSelector)
        where TSource : notnull
        => source.ToDictionary(Identity, valueSelector);
}
