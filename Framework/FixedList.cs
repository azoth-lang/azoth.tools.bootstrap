using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework;

public interface IFixedList<out T> : IReadOnlyList<T>
{
    //[Obsolete("Not supported", error: true)]
    //bool Equals(object? other);

    //[Obsolete("Not supported", error: true)]
    //int GetHashCode();

    //bool Equals<TItem>(IFixedList<TItem>? other) where TItem : IEquatable<TItem>;

    //public bool Equals(IFixedList<T>? other)
    //    => this.Equals(other, EqualityComparer<T>.Default);

    //abstract static bool operator ==(IFixedList<T>? left, IFixedList<T>? right)
    //    => FixedList.Equals(left, right);

    //abstract static bool operator !=(IFixedList<T>? left, IFixedList<T>? right)
    //     => !FixedList.Equals(left, right);
}

// These attributes make it so FixedList<T> is displayed nicely in the debugger similar to List<T>
[DebuggerDisplay("Count = {" + nameof(Count) + "}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
public sealed class FixedList<T> : IFixedList<T>//, IEquatable<IFixedList<T>>
                                                //where T : IEquatable<T>
{
    public static readonly FixedList<T> Empty = new(Enumerable.Empty<T>());

    private readonly IReadOnlyList<T> items;

    [DebuggerStepThrough]
    public FixedList(IEnumerable<T> items)
    {
        // Don't use `AsReadOnly` because FixedList<T> is already a wrapper
        this.items = items.ToList();
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
    //    [Obsolete("Not supported", error: true)]
    //#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public override bool Equals(object? obj)
        //#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        => throw new NotSupportedException();
    //=> obj is IFixedList<T> other && Equals(other);

    //public bool Equals<TItem>(IFixedList<TItem>? other)
    //    where TItem : IEquatable<TItem>
    //{
    //    if (other is null || Count != other.Count) return false;

    //    if (other is IFixedList<T> otherAdapted)
    //        return FixedList.Equals(this, otherAdapted);

    //    if (this is IFixedList<TItem> thisAdapted)
    //        return FixedList.Equals(thisAdapted, other);

    //    return false;
    //}

    //public bool Equals(IFixedList<T>? other)
    //{
    //    if (other is null || Count != other.Count) return false;
    //    var count = Count;
    //    var comparer = EqualityComparer<T>.Default;
    //    for (int i = 0; i < count; i++)
    //        if (!comparer.Equals(items[i], other[i]))
    //            return false;
    //    return true;
    //}

    //[Obsolete("Not supported", error: true)]
    //#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public override int GetHashCode() //#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
    {
        var comparer = StrictEqualityComparer<T>.Instance;
        HashCode hash = new HashCode();
        hash.Add(Count);
        foreach (var item in items) hash.Add(item, comparer);
        return hash.ToHashCode();
    }
    #endregion
}

public static class FixedList
{
    public static FixedList<T> Create<T>(params T[] items)
        => new(items);

    public static bool ItemsEquals<T>(this IFixedList<T> first, IFixedList<T>? second)
        where T : IEquatable<T>
        => first.ItemsEquals(second, EqualityComparer<T>.Default);

    public static bool ItemsEquals<T>(this IFixedList<T> first, IFixedList<T>? second, IEqualityComparer<T> comparer)
    {
        if (ReferenceEquals(first, second)) return true;
        if (first.Count != second?.Count) return false;

        var count = first.Count;
        for (int i = 0; i < count; i++)
            if (!comparer.Equals(first[i], second[i]))
                return false;
        return true;
    }
}
