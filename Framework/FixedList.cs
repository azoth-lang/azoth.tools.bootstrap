using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Framework;

public interface IFixedList : IEnumerable
{
    int Count { get; }
    bool IsEmpty { get; }

    object? this[int index] { get; }
}

[CollectionBuilder(typeof(FixedList), "Create")]
public interface IFixedList<out T> : IFixedList, IReadOnlyList<T>
{
    new int Count { get; }
    int IFixedList.Count => Count;
    int IReadOnlyCollection<T>.Count => Count;
    new T this[int index] { get; }
    object? IFixedList.this[int index] => this[index];
    T IReadOnlyList<T>.this[int index] => this[index];

    public IFixedList<TOther>? As<TOther>()
        where TOther : class
    {
        if (this is IFixedList<TOther> other) return other;
        var items = new TOther[Count];
        for (int i = 0; i < Count; i++)
        {
            T item = this[i];
            if (item is not null)
            {
                if (item is not TOther casted)
                    return null;
                items[i] = casted;
            }
        }
        return FixedList.Create(items);
    }

    protected static bool Equals(IFixedList<T> self, object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(self, obj)) return true;
        return obj switch
        {
            IFixedList<T> other => Equals(self, other),
            IFixedList other => Equals(self, other),
            _ => false
        };
    }

    private static bool Equals(IFixedList<T> self, IFixedList<T> other)
    {
        if (self.Count != other.Count) return false;
        var comparer = EqualityComparer<T>.Default;
        return self.SequenceEqual(other, comparer);
    }

    private static bool Equals(IFixedList<T> self, IFixedList other)
    {
        var count = self.Count;
        if (count != other.Count) return false;
        for (int i = 0; i < count; i++)
            if (!Equals(self[i], other[i]))
                return false;
        return true;
    }

    protected static int ComputeHashCode(IFixedList<T> self)
    {
        HashCode hash = new HashCode();
        hash.Add(self.Count);
        foreach (var item in self) hash.Add(item);
        return hash.ToHashCode();
    }
}

public static class FixedList
{
    public static IFixedList<T> Empty<T>()
        => Of<T>.Empty;

    public static IFixedList<T> Create<T>(ReadOnlySpan<T> items)
        => items.IsEmpty ? Of<T>.Empty : new(items);

    public static IFixedList<T> Create<T>(IEnumerable<T> items)
        => new Of<T>(items);

    public static IFixedList<T> Create<T>(params T[] items)
        // Must use AsSpan in case the array was passed in so that a new array is allocated.
        => new Of<T>(items.AsSpan());

    public static IEqualityComparer<IFixedList<T>> EqualityComparer<T>()
        where T : IEquatable<T>
        => System.Collections.Generic.EqualityComparer<IFixedList<T>>.Default;

    public static int? IndexOf<T>(this IFixedList<T> list, T item)
        => IndexOf(list, item, System.Collections.Generic.EqualityComparer<T>.Default);

    public static int? IndexOf<T>(IFixedList<T> list, T item, IEqualityComparer<T> comparer)
    {
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
        public Of(ReadOnlySpan<T> items)
        {
            // Don't use `AsReadOnly` because FixedList<T> is already a wrapper. Use `ToArray` to
            // avoid allocating any more memory than necessary.
            this.items = items.ToArray();
        }

        [DebuggerStepThrough]
        internal Of(IEnumerable<T> items)
        {
            // Don't use `AsReadOnly` because FixedList<T> is already a wrapper. Use `ToArray` to
            // avoid allocating any more memory than necessary.
            this.items = items.ToArray();
        }

        /// <summary>
        /// CAUTION: This constructor is for internal use only. It does not copy the items to a new collection.
        /// </summary>
        [DebuggerStepThrough]
        internal Of(T[] items)
        {
            // Don't use `AsReadOnly` because FixedList<T> is already a wrapper. Use `ToArray` to
            // avoid allocating any more memory than necessary.
            this.items = items;
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
        public override bool Equals(object? obj) => IFixedList<T>.Equals(this, obj);

        public override int GetHashCode()
            => hashCode != 0 ? hashCode : hashCode = IFixedList<T>.ComputeHashCode(this);
        #endregion
    }
}
