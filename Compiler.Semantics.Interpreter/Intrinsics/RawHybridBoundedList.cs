using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Intrinsics;

internal abstract class RawHybridBoundedList : IList<AzothValue>, IIntrinsicValue
{
    public static RawHybridBoundedList Create(Type itemType, bool ensurePrefixZeroed, nuint capacity)
    {
        // Parameter is unused since C# provides no way to not initialize the field to zero
        _ = ensurePrefixZeroed;

        if (itemType.Equals(Type.Byte))
        {
            // GC.AllocateArray allocates an uninitialized array
            var items = ensurePrefixZeroed ? new byte[capacity] : GC.AllocateArray<byte>((int)capacity);
            return new Bytes(items, 0);
        }
        else
        {
            // Note that GC.AllocateArray probably isn't generating an uninitialized array because AzothValue contains
            var items = ensurePrefixZeroed ? new AzothValue[capacity] : GC.AllocateArray<AzothValue>((int)capacity);
            return new Values(items, 0);
        }
    }

    public static RawHybridBoundedList Create(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        return new Bytes(bytes, (nuint)bytes.Length);
    }

    protected RawHybridBoundedList(nuint count)
    {
        Count = count;
    }

    public AzothValue Prefix { get; set; }
    public nuint Count { get; protected set; }
    public abstract nuint Capacity { get; }
    public abstract void Add(AzothValue value);
    public abstract AzothValue Get(nuint index);
    public abstract void Set(nuint index, AzothValue value);
    public abstract void Shrink(nuint count);

    public abstract string GetStringFromUtf8Bytes(nuint start, nuint byteCount);

    #region IList<T>
    int ICollection<AzothValue>.Count => (int)Count;

    AzothValue IList<AzothValue>.this[int index]
    {
        get => Get((nuint)index);
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

    private abstract class Base<T> : RawHybridBoundedList
        where T : struct
    {
        protected Base(T[] items, nuint count, bool clearValues)
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
            if (Count >= Capacity) throw new AbortException("Cannot add to Raw_Bounded_List.");
            items[Count++] = value;
        }

        protected T ValueAt(nuint index)
        {
            // Even though this is raw and not supposed to check bounds, aborting makes debugging easier
            if (index >= Count) throw new AbortException("Index out of bounds");
            return items[index];
        }

        protected void SetValue(nuint index, T value) => items[index] = value;

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

    private sealed class Bytes : Base<byte>
    {
        public Bytes(byte[] bytes, nuint count)
            : base(bytes, count, false) { }

        public override void Add(AzothValue value) => AddValue(value.ByteValue);
        public override AzothValue Get(nuint index) => AzothValue.Byte(ValueAt(index));
        public override void Set(nuint index, AzothValue value) => SetValue(index, value.ByteValue);

        public override string GetStringFromUtf8Bytes(nuint start, nuint byteCount)
            => Encoding.UTF8.GetString(AsSpan().Slice((int)start, (int)byteCount));
    }

    private sealed class Values : Base<AzothValue>
    {
        public Values(AzothValue[] values, nuint count)
            : base(values, count, false) { }

        public override void Add(AzothValue value) => AddValue(value);
        public override AzothValue Get(nuint index) => ValueAt(index);
        public override void Set(nuint index, AzothValue value) => SetValue(index, value);

        public override string GetStringFromUtf8Bytes(nuint start, nuint byteCount)
            => throw new NotSupportedException("List is not a list of bytes.");
    }
}

