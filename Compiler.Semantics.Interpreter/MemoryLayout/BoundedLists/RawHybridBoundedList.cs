using System;
using System.Collections;
using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout.BoundedLists;

internal abstract class RawHybridBoundedList : IList<AzothValue>, IIntrinsicValue
{
    protected RawHybridBoundedList(nuint count)
    {
        Count = count;
    }

    public AzothValue Fixed { get; set; }
    public nuint Count { get; protected set; }
    public abstract nuint Capacity { get; }
    public abstract void Add(AzothValue value);
    public AzothRef RefAt(nuint index) => new(this, (int)index);
    public abstract AzothValue At(nuint index);
    public abstract void Set(nuint index, AzothValue value);
    public abstract void Shrink(nuint count);

    #region IList<T>
    int ICollection<AzothValue>.Count => (int)Count;

    AzothValue IList<AzothValue>.this[int index]
    {
        get => At((nuint)index);
        set => Set((nuint)index, value);
    }
    #endregion

    #region IList<T> Not Supported
    bool ICollection<AzothValue>.IsReadOnly => false;
    void ICollection<AzothValue>.Add(AzothValue item) => throw new NotSupportedException();
    void ICollection<AzothValue>.Clear() => throw new NotSupportedException();
    bool ICollection<AzothValue>.Contains(AzothValue item) => throw new NotSupportedException();
    void ICollection<AzothValue>.CopyTo(AzothValue[] array, int arrayIndex) => throw new NotSupportedException();
    bool ICollection<AzothValue>.Remove(AzothValue item) => throw new NotSupportedException();
    IEnumerator<AzothValue> IEnumerable<AzothValue>.GetEnumerator() => throw new NotSupportedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
    int IList<AzothValue>.IndexOf(AzothValue item) => throw new NotSupportedException();
    void IList<AzothValue>.Insert(int index, AzothValue item) => throw new NotSupportedException();
    void IList<AzothValue>.RemoveAt(int index) => throw new NotSupportedException();
    #endregion
}

internal abstract class RawHybridBoundedListBase<T> : RawHybridBoundedList, IList<AzothValue>
    where T : struct
{
    protected RawHybridBoundedListBase(nuint capacity, bool clearValues)
        : base(0)
    {
        items = new T[capacity];
        this.clearValues = clearValues;
    }

    protected RawHybridBoundedListBase(T[] items, nuint count, bool clearValues)
        : base(count)
    {
        if ((int)count > items.Length)
            throw new ArgumentOutOfRangeException(nameof(count), count, "Must be less than or equal to capacity");
        this.items = items;
        this.clearValues = clearValues;
    }

    public override nuint Capacity => (nuint)items.Length;
    private readonly T[] items;
    private readonly bool clearValues;

    protected ReadOnlySpan<T> AsSpan() => items;

    protected void AddValue(T value)
    {
        // Even though this is raw and not supposed to check bounds, aborting makes debugging easier
        if (Count >= Capacity)
            throw new AbortException("Cannot add to Raw_Bounded_List.");
        items[Count++] = value;
    }

    public T ValueAt(nuint index)
    {
        // Even though this is raw and not supposed to check bounds, aborting makes debugging easier
        if (index >= Count)
            throw new AbortException("Index out of bounds");
        return items[index];
    }

    public void SetValue(nuint index, T value) => items[index] = value;

    public override void Shrink(nuint count)
    {
        if (count > Count) throw new AbortException("Cannot increase Raw_Bounded_List size with `shrink`.");
        var oldCount = Count;
        Count = count;
        if (!clearValues) return;
        // To allow the .NET garbage collector to see collect referenced values, clear that part of
        // the list.
        for (nuint i = count; i < oldCount; i++)
            items[i] = default;
    }
}
