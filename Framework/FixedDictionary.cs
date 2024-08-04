using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Framework;

[DebuggerDisplay("Count = {" + nameof(Count) + "}")]
[DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
public class FixedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IEquatable<FixedDictionary<TKey, TValue>>
    where TKey : notnull
{
    public static readonly FixedDictionary<TKey, TValue> Empty = new(new Dictionary<TKey, TValue>());

    private readonly IReadOnlyDictionary<TKey, TValue> items;

    [DebuggerStepThrough]
    public FixedDictionary(IDictionary<TKey, TValue> items)
    {
        // Don't wrap in ReadOnlyDictionary because FixedDictionary<TKey, TValue> is already a wrapper
        this.items = new Dictionary<TKey, TValue>(items);
    }

    [DebuggerStepThrough]
    public FixedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        // Don't wrap in ReadOnlyDictionary because FixedDictionary<TKey, TValue> is already a wrapper
        this.items = new Dictionary<TKey, TValue>(items);
    }

    [DebuggerStepThrough]
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => items.GetEnumerator();

    [DebuggerStepThrough]
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)items).GetEnumerator();

    public int Count
    {
        [DebuggerStepThrough]
        get => items.Count;
    }

    public bool IsEmpty => items.Count == 0;

    [DebuggerStepThrough]
    public bool ContainsKey(TKey key) => items.ContainsKey(key);

    [DebuggerStepThrough]
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        => items.TryGetValue(key, out value);

    public TValue this[TKey key]
    {
        [DebuggerStepThrough]
        get => items[key];
    }

    public IEnumerable<TKey> Keys
    {
        [DebuggerStepThrough]
        get => items.Keys;
    }

    public IEnumerable<TValue> Values
    {
        [DebuggerStepThrough]
        get => items.Values;
    }

    #region Equality
    public bool Equals(FixedDictionary<TKey, TValue>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        if (Count != other.Count) return false;
        var valueComparer = EqualityComparer<TValue>.Default;
        foreach (var (otherKey, otherValue) in other)
            if (!TryGetValue(otherKey, out var value) || !valueComparer.Equals(value, otherValue))
                return false;
        return true;
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is FixedDictionary<TKey, TValue> other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Count);
        int hashCode = 0;
        foreach (var item in items)
            hashCode ^= GetHashCode(item);
        hash.Add(hashCode);
        return hashCode;
    }

    private static int GetHashCode(KeyValuePair<TKey, TValue> item)
        => HashCode.Combine(item.Key, item.Value);
    #endregion
}
