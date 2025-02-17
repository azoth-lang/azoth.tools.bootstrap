using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Azoth.Tools.Bootstrap.Framework.Functions;

namespace Azoth.Tools.Bootstrap.Framework;

public static class DictionaryExtensions
{
    /// <summary>
    /// Adds a key/value pair to the <see cref="Dictionary{TKey,TValue}"/> by using the specified
    /// function if the key does not already exist. Returns the new value, or the existing value if
    /// the key exists.
    /// </summary>
    /// <remarks>This method is not thread-safe.</remarks>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="Dictionary{TKey,TValue}"/>.</exception>
    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> valueFactory)
        where TKey : notnull
    {
        ref TValue? location = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
        if (exists)
            return location!;
        var value = valueFactory(key);
        location = value;
        return value;
    }

    /// <summary>
    /// Adds a key/value pair to the <see cref="Dictionary{TKey,TValue}"/> by using the specified
    /// function if the key does not already exist. Returns the new value, or the existing value if
    /// the key exists.
    /// </summary>
    /// <returns><see langword="true"/> if the key/value pair was added to the dictionary
    /// successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> valueFactory)
        where TKey : notnull
    {
        ref TValue? location = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
        if (exists)
            return false;

        location = valueFactory(key);
        return true;
    }

    /// <summary>
    /// Try to update an existing entry in a dictionary. If no entry exists, do nothing.
    /// </summary>
    /// <remarks>This provides a more efficient operation than first checking
    /// <see cref="Dictionary{TKey,TValue}.ContainsKey"/> and then setting
    /// <see cref="Dictionary{TKey,TValue}.this"/>.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull
    {
        ref TValue location = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
        if (!Unsafe.IsNullRef(ref location))
        {
            location = value;
            return true;
        }
        return false;
    }

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
