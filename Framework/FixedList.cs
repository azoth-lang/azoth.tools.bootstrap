using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework;

public interface IFixedList<out T> : IReadOnlyList<T>
{
}

public static class FixedList
{
    public static IFixedList<T> Empty<T>()
        => Of<T>.Empty;

    public static IFixedList<T> Create<T>(IEnumerable<T> items)
        => new Of<T>(items);

    public static IFixedList<T> Create<T>(params T[] items)
        => new Of<T>(items);

    public static bool ItemsEqual<T>(this IFixedList<T> first, IFixedList<T>? second)
        where T : IEquatable<T>
        => first.ItemsEqual(second, EqualityComparer<T>.Default);

    public static bool ItemsEqual<T>(this IFixedList<T> first, IFixedList<T>? second, IEqualityComparer<T> comparer)
    {
        if (ReferenceEquals(first, second)) return true;
        if (first.Count != second?.Count) return false;

        var count = first.Count;
        for (int i = 0; i < count; i++)
            if (!comparer.Equals(first[i], second[i]))
                return false;
        return true;
    }

    // These attributes make it so FixedList.Of<T> is displayed nicely in the debugger similar to List<T>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    private sealed class Of<T> : IFixedList<T>
    {
        public static readonly Of<T> Empty = new(Enumerable.Empty<T>());

        private readonly IReadOnlyList<T> items;

        [DebuggerStepThrough]
        internal Of(IEnumerable<T> items)
        {
            // Don't use `AsReadOnly` because FixedList<T> is already a wrapper. Use `ToArray` to
            // avoid allocating any more memory than necessary.
            this.items = items.ToArray();
        }

        [DebuggerStepThrough]
        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)items).GetEnumerator();

        public int Count
        {
            [DebuggerStepThrough]
            get => items.Count;
        }

        public T this[int index]
        {
            [DebuggerStepThrough]
            get => items[index];
        }

        #region Equality
        public override bool Equals(object? obj) => throw new NotSupportedException();

        public override int GetHashCode()
        {
            var comparer = StrictEqualityComparer<T>.Instance;
            HashCode hash = new HashCode();
            hash.Add(Count);
            foreach (var item in items) hash.Add(item, comparer);
            return hash.ToHashCode();
        }
        #endregion
    }
}
