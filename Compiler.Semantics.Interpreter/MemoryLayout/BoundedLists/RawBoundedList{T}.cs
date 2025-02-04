using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout.BoundedLists;

internal abstract class RawBoundedList<T> : IRawBoundedList
    where T : struct
{
    protected RawBoundedList(nuint capacity, bool clearValues)
    {
        Count = 0;
        items = new T[capacity];
        this.clearValues = clearValues;
    }

    protected RawBoundedList(T[] items, nuint count, bool clearValues)
    {
        if ((int)count > items.Length)
            throw new ArgumentOutOfRangeException(nameof(count), count, "Must be less than or equal to capacity");
        this.items = items;
        Count = count;
        this.clearValues = clearValues;
    }

    public AzothValue Fixed { get; set; }
    public nuint Count { get; private set; }
    public nuint Capacity => (nuint)items.Length;
    private readonly T[] items;
    private readonly bool clearValues;
    protected ReadOnlySpan<T> AsSpan() => items;

    public abstract void Add(AzothValue value);
    protected void AddValue(T value)
    {
        // Even though this is raw and not supposed to check bounds, aborting makes debugging easier
        if (Count >= Capacity)
            throw new AbortException("Cannot add to Raw_Bounded_List.");
        items[Count++] = value;
    }

    public abstract AzothValue At(nuint index);
    public T ValueAt(nuint index)
    {
        // Even though this is raw and not supposed to check bounds, aborting makes debugging easier
        if (index >= Count)
            throw new AbortException("Index out of bounds");
        return items[index];
    }

    public abstract void Set(nuint index, AzothValue value);
    public void SetValue(nuint index, T value) => items[index] = value;

    public void Shrink(nuint count)
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
