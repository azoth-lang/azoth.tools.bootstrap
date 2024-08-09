using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework;

public interface IFixedList<out T> : IReadOnlyList<T>
{
    bool IsEmpty { get; }

    bool Equals(IFixedList<object?>? other);

    static IEqualityComparer<IFixedList<T>> EqualityComparer => EqualityComparer<IFixedList<T>>.Default;
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
        => first.ItemsEqual(second, System.Collections.Generic.EqualityComparer<T>.Default);

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

    public static IEqualityComparer<IFixedList<object?>> ObjectEqualityComparer
        => System.Collections.Generic.EqualityComparer<IFixedList<object?>>.Default;

    public static IEqualityComparer<IFixedList<T>> EqualityComparer<T>()
        where T : IEquatable<T>
        => System.Collections.Generic.EqualityComparer<IFixedList<T>>.Default;

    public static int? IndexOf<T>(this IFixedList<T> list, T item)
    {
        var comparer = System.Collections.Generic.EqualityComparer<T>.Default;
        for (int i = 0; i < list.Count; i++)
            if (comparer.Equals(list[i], item))
                return i;
        return null;
    }

    // These attributes make it so FixedList.Of<T> is displayed nicely in the debugger similar to List<T>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    private sealed class Of<T> : IFixedList<T>
    {
        public static readonly Of<T> Empty = new([]);

        private readonly IReadOnlyList<T> items;
        private int hashCode;

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

        public bool IsEmpty => Count == 0;

        public T this[int index]
        {
            [DebuggerStepThrough]
            get => items[index];
        }

        #region Equality
        public bool Equals(IFixedList<object?>? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (Count != other.Count || GetHashCode() != other.GetHashCode()) return false;
            for (int i = 0; i < Count; i++)
                if (!Equals(this[i], other[i]))
                    return false;
            return true;
        }

        public override bool Equals(object? obj)
            => ReferenceEquals(this, obj) || obj is IFixedList<object?> other && Equals(other);

        public override int GetHashCode() => hashCode != 0 ? hashCode : hashCode = ComputeHashCode();

        private int ComputeHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Count);
            foreach (var item in items)
                hash.Add(item);
            return hash.ToHashCode();
        }
        #endregion
    }
}
