using System;
using System.Collections;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Intrinsics;

internal abstract class RawHybridArray : IIntrinsicValue, IList<Value>
{
    public static RawHybridArray Create(
        Type prefixType,
        Type itemType,
        bool ensurePrefixZeroed,
        UIntPtr count,
        bool ensureZeroed)
    {
        // Parameter is unused since C# provides no way to not initialize the field to zero
        _ = ensurePrefixZeroed;

        if (prefixType.Equals(Type.Void) && itemType.Equals(Type.Byte))
        {
            // GC.AllocateArray allocates an uninitialized array
            var items = ensurePrefixZeroed ? new byte[count] : GC.AllocateArray<byte>((int)count);
            return new Bytes(items);
        }
        else
        {
            // Note that GC.AllocateArray probably isn't generating an uninitialized array because Value contains a reference
            var items = ensurePrefixZeroed ? new Value[count] : GC.AllocateArray<Value>((int)count);
            return new Values(items);
        }
    }

    public Value Prefix { get; set; }
    public abstract nuint Count { get; }

    public abstract Value Get(nuint index);
    public abstract void Set(nuint index, Value value);

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

    private abstract class Base<T> : RawHybridArray
        where T : struct
    {
        protected Base(T[] items)
        {
            this.items = items;
        }

        public override nuint Count => (nuint)items.Length;
        private readonly T[] items;

        protected T ValueAt(nuint index)
        {
            // Even though this is raw and not supposed to check bounds, aborting makes debugging easier
            if (index >= Count)
                throw new AbortException("Index out of bounds");
            return items[index];
        }

        protected void SetValue(nuint index, T value) => items[index] = value;
    }

    private sealed class Bytes : Base<byte>
    {
        public Bytes(byte[] items)
            : base(items) { }

        public override Value Get(nuint index) => Value.FromByte(ValueAt(index));
        public override void Set(nuint index, Value value) => SetValue(index, value.Byte);
    }

    private sealed class Values : Base<Value>
    {
        public Values(Value[] items)
            : base(items) { }

        public override Value Get(nuint index) => ValueAt(index);
        public override void Set(nuint index, Value value) => SetValue(index, value);
    }
}
