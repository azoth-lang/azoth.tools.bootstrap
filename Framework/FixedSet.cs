using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Framework;

[CollectionBuilder(typeof(FixedSet), "Create")]
public interface IFixedSet<out T> : IReadOnlyCollection<T>;

public static class FixedSet
{
    public static IFixedSet<T> Empty<T>() => Of<T>.Empty;

    public static IFixedSet<T> Create<T>(ReadOnlySpan<T> items)
        => items.IsEmpty ? Of<T>.Empty : new(items);

    public static IFixedSet<T> Create<T>(IEnumerable<T> items) => new Of<T>(items);

    public static IFixedSet<T> Create<T>(params T[] items)
        => items.IsEmpty() ? Of<T>.Empty : new(items.AsSpan());

    public static bool ItemsEqual<T>(this IFixedSet<T> first, IFixedSet<T>? second)
        where T : IEquatable<T>
    {
        if (ReferenceEquals(first, second)) return true;
        if (first.Count != second?.Count) return false;

        foreach (var item in first)
            if (!second.Contains(item))
                return false;
        return true;
    }

    public static bool Contains<T>(this IFixedSet<T> set, T value)
        => ((Of)set).Contains(value);

    public static IEqualityComparer<IFixedSet<T>> ItemComparer<T>()
        where T : IEquatable<T>
        => ItemEqualityComparer<T>.Instance;

    public static IFixedSet<T> Union<T>(this IFixedSet<T> set, IEnumerable<T> other)
    {
        HashSet<T>? newSet = null;
        foreach (var item in other)
        {
            if (newSet is not null)
                newSet.Add(item);
            else if (!set.Contains(item))
                newSet = [.. set, item];
        }
        return newSet is null ? set : new Of<T>(newSet);
    }

    private abstract class Of
    {
        public abstract bool Contains(object? item);
    }

    // These attributes make it so FixedSet.Of<T> is displayed nicely in the debugger similar to Set<T>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    private sealed class Of<T> : Of, IFixedSet<T>
    {
        public static readonly Of<T> Empty = new Of<T>([]);

        private readonly IReadOnlySet<T> items;

        [DebuggerStepThrough]
        public Of(ReadOnlySpan<T> items)
        {
            var set = new HashSet<T>(items.Length);
            foreach (var item in items)
                set.Add(item);
            this.items = set;
        }

        [DebuggerStepThrough]
        public Of(IEnumerable<T> items)
        {
            this.items = items.ToHashSet();
        }

        /// <summary>
        /// CAUTION: This constructor is for internal use only. It does not copy the items.
        /// </summary>
        [DebuggerStepThrough]
        internal Of(IReadOnlySet<T> items)
        {
            this.items = items;
        }

        [DebuggerStepThrough]
        public Of(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            this.items = items.ToHashSet(equalityComparer);
        }

        [DebuggerStepThrough]
        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)items).GetEnumerator();

        public int Count => items.Count;

        public override bool Contains(object? item) => item is T value && items.Contains(value);
    }

    private static class ItemEqualityComparer<T>
        where T : IEquatable<T>
    {
        public static readonly IEqualityComparer<IFixedSet<T>> Instance
            = EqualityComparer<IFixedSet<T>>.Create((l, r) => ReferenceEquals(l, r) || (l?.ItemsEqual(r) ?? false),
                GetHashCode);

        private static int GetHashCode(IFixedSet<T> set)
        {
            var comparer = StrictEqualityComparer<T>.Instance;
            HashCode hash = new HashCode();
            hash.Add(set.Count);
            // Xor the hash codes so the order doesn't matter
            int itemHash = 0;
            foreach (var item in set)
                itemHash ^= comparer.GetHashCode(item);
            hash.Add(itemHash);
            return hash.ToHashCode();
        }
    }
}
