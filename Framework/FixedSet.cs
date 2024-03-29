using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework;

public interface IFixedSet<out T> : IReadOnlyCollection<T>
{
}

public static class FixedSet
{
    public static IFixedSet<T> Empty<T>() => Of<T>.Empty;

    public static IFixedSet<T> Create<T>(IEnumerable<T> items) => new Of<T>(items);

    public static IFixedSet<T> Create<T>(params T[] items) => new Of<T>(items);

    // These attributes make it so FixedSet.Of<T> is displayed nicely in the debugger similar to Set<T>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    private sealed class Of<T> : IFixedSet<T>
    {
        public static readonly Of<T> Empty = new Of<T>(Enumerable.Empty<T>());

        private readonly IReadOnlySet<T> items;

        [DebuggerStepThrough]
        public Of(IEnumerable<T> items)
        {
            this.items = items.ToHashSet();
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

        #region Equality
        public override bool Equals(object? obj) => throw new NotSupportedException();

        public override int GetHashCode()
        {
            var comparer = StrictEqualityComparer<T>.Instance;
            HashCode hash = new HashCode();
            hash.Add(Count);
            // Order the hash codes so there is a consistent hash order
            foreach (var itemHash in items.Select(i => comparer.GetHashCode(i!)).OrderBy(h => h))
                hash.Add(itemHash);
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
}
