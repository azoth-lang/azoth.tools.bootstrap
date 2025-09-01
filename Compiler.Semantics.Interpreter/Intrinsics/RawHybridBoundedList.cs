using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Intrinsics;

internal abstract class RawHybridBoundedList : IList<Value>, IIntrinsicValue
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
            // Note that GC.AllocateArray probably isn't generating an uninitialized array because Value contains
            var items = ensurePrefixZeroed ? new Value[capacity] : GC.AllocateArray<Value>((int)capacity);
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

    public Value Prefix { get; set; }
    public nuint Count { get; protected set; }
    public abstract nuint Capacity { get; }
    public abstract void Add(Value value);
    public abstract Value Get(nuint index);
    public abstract void Set(nuint index, Value value);
    public abstract void Shrink(nuint count);

    public abstract string GetStringFromUtf8Bytes(nuint start, nuint byteCount);

    #region IList<T>
    int ICollection<Value>.Count => (int)Count;

    Value IList<Value>.this[int index]
    {
        get => Get((nuint)index);
        set => Set((nuint)index, value);
    }
    #endregion

    #region IList<T> Not Supported
    bool ICollection<Value>.IsReadOnly => false;
    void ICollection<Value>.Add(Value item) => throw new NotSupportedException();
    void ICollection<Value>.Clear() => throw new NotSupportedException();
    bool ICollection<Value>.Contains(Value item) => throw new NotSupportedException();
    void ICollection<Value>.CopyTo(Value[] array, int arrayIndex) => throw new NotSupportedException();
    bool ICollection<Value>.Remove(Value item) => throw new NotSupportedException();
    IEnumerator<Value> IEnumerable<Value>.GetEnumerator() => throw new NotSupportedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
    int IList<Value>.IndexOf(Value item) => throw new NotSupportedException();
    void IList<Value>.Insert(int index, Value item) => throw new NotSupportedException();
    void IList<Value>.RemoveAt(int index) => throw new NotSupportedException();
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

        public override void Add(Value value) => AddValue(value.Byte);
        public override Value Get(nuint index) => Value.FromByte(ValueAt(index));
        public override void Set(nuint index, Value value) => SetValue(index, value.Byte);

        public override string GetStringFromUtf8Bytes(nuint start, nuint byteCount)
            => Encoding.UTF8.GetString(AsSpan().Slice((int)start, (int)byteCount));
    }

    private sealed class Values : Base<Value>
    {
        public Values(Value[] values, nuint count)
            : base(values, count, false) { }

        public override void Add(Value value) => AddValue(value);
        public override Value Get(nuint index) => ValueAt(index);
        public override void Set(nuint index, Value value) => SetValue(index, value);

        public override string GetStringFromUtf8Bytes(nuint start, nuint byteCount)
            => throw new NotSupportedException("List is not a list of bytes.");
    }
}

