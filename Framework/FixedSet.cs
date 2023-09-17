using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework;

// These attributes make it so FixedSet<T> is displayed nicely in the debugger similar to Set<T>
[DebuggerDisplay("Count = {" + nameof(Count) + "}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
public class FixedSet<T> : IReadOnlySet<T>, IEquatable<FixedSet<T>>
{
    public static readonly FixedSet<T> Empty = new FixedSet<T>(Enumerable.Empty<T>());

    private readonly IReadOnlySet<T> items;

    [DebuggerStepThrough]
    public FixedSet(IEnumerable<T> items)
    {
        this.items = items.ToHashSet();
    }

    [DebuggerStepThrough]
    public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

    [DebuggerStepThrough]
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)items).GetEnumerator();

    public int Count => items.Count;

    #region Equality
    public override bool Equals(object? obj) => Equals(obj as FixedSet<T>);

    public bool Equals(FixedSet<T>? other)
        => other is not null && Count == other.Count && items.SetEquals(other.items);

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        // Order the hash codes so there is a consistent hash order
        foreach (var item in items.OrderBy(i => i?.GetHashCode()))
            hash.Add(item);
        return hash.ToHashCode();
    }
    #endregion

    public bool Contains(T item) => items.Contains(item);

    public bool IsProperSubsetOf(IEnumerable<T> other) => items.IsProperSubsetOf(other);

    public bool IsProperSupersetOf(IEnumerable<T> other) => items.IsProperSupersetOf(other);

    public bool IsSubsetOf(IEnumerable<T> other) => items.IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<T> other) => items.IsSupersetOf(other);

    public bool Overlaps(IEnumerable<T> other) => items.Overlaps(other);

    public bool SetEquals(IEnumerable<T> other) => items.SetEquals(other);
}

public static class FixedSet
{
    public static FixedSet<T> Create<T>(params T[] items) => new(items);
}
