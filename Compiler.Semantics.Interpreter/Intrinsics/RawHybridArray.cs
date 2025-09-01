using System;
using System.Collections;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Intrinsics;

internal abstract class RawHybridArray : IIntrinsicValue, IList<AzothValue>
{
    public static RawHybridArray Create(Type itemType, bool ensurePrefixZeroed, nuint count, bool ensureZeroed)
    {
        // Parameter is unused since C# provides no way to not initialize the field to zero
        _ = ensurePrefixZeroed;

        if (itemType.Equals(Type.Byte))
        {
            // GC.AllocateArray allocates an uninitialized array
            var items = ensurePrefixZeroed ? new byte[count] : GC.AllocateArray<byte>((int)count);
            return new Bytes(items);
        }
        else
        {
            // Note that GC.AllocateArray probably isn't generating an uninitialized array because AzothValue contains
            var items = ensurePrefixZeroed ? new AzothValue[count] : GC.AllocateArray<AzothValue>((int)count);
            return new Values(items);
        }
    }

    public AzothValue Prefix { get; set; }
    public abstract nuint Count { get; }

    public abstract AzothValue Get(nuint index);
    public abstract void Set(nuint index, AzothValue value);

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

        public override AzothValue Get(nuint index) => AzothValue.Byte(ValueAt(index));
        public override void Set(nuint index, AzothValue value) => SetValue(index, value.ByteValue);
    }

    private sealed class Values : Base<AzothValue>
    {
        public Values(AzothValue[] items)
            : base(items) { }

        public override AzothValue Get(nuint index) => ValueAt(index);
        public override void Set(nuint index, AzothValue value) => SetValue(index, value);
    }
}
