using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Framework;

[CollectionBuilder(typeof(FixedSet), "Create")]
public interface IFixedSet<out T> : IReadOnlyCollection<T>
{
    bool IsEmpty { get; }

    bool Equals(IFixedSet<object?>? other);
}

public static class FixedSet
{
    public static IFixedSet<T> Empty<T>() => Of<T>.Empty;

    public static IFixedSet<T> Create<T>(ReadOnlySpan<T> items)
        => items.IsEmpty ? Of<T>.Empty : new(items);

    public static IFixedSet<T> Create<T>(IEnumerable<T> items)
        => new Of<T>(items);

    public static IFixedSet<T> Create<T>(params T[] items)
        => items.IsEmpty() ? Of<T>.Empty : new(items.AsSpan());

    public static bool Contains<T>(this IFixedSet<T> set, T value)
        => ((Of)set).Contains(value);

    public static IEqualityComparer<IFixedSet<object?>> ObjectEqualityComparer
        => System.Collections.Generic.EqualityComparer<IFixedSet<object?>>.Default;

    public static IEqualityComparer<IFixedSet<T>> EqualityComparer<T>()
        where T : IEquatable<T>
        => System.Collections.Generic.EqualityComparer<IFixedSet<T>>.Default;

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
        private int hashCode;

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
        /// CAUTION: This constructor is for internal use only. It does not copy the items to a new collection.
        /// </summary>
        [DebuggerStepThrough]
        internal Of(IReadOnlySet<T> items)
        {
            this.items = items;
        }

        [DebuggerStepThrough]
        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)items).GetEnumerator();

        public int Count => items.Count;

        public bool IsEmpty => items.Count == 0;

        public override bool Contains(object? item) => item is T value && items.Contains(value);

        #region Equality
        public bool Equals(IFixedSet<object?>? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (Count != other.Count || GetHashCode() != other.GetHashCode()) return false;
            foreach (var item in other)
                if (!Contains(item))
                    return false;
            return true;
        }

        public override bool Equals(object? obj)
            // TODO handle equality like IFixedList<T> does so that it works for all types in all cases
            => ReferenceEquals(this, obj) || obj is IFixedSet<object?> other && Equals(other);

        public override int GetHashCode() => hashCode != 0 ? hashCode : hashCode = ComputeHashCode();

        private int ComputeHashCode()
        {
            var unorderedHash = new UnorderedHashCode();
            foreach (var item in this)
                unorderedHash.Add(item);
            return unorderedHash.ToHashCode();
        }
        #endregion
    }
}
