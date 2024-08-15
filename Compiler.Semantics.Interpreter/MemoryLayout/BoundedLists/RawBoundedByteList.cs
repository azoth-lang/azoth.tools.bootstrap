using System.Text;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout.BoundedLists;

internal sealed class RawBoundedByteList : RawBoundedList<byte>
{
    public RawBoundedByteList(nuint capacity)
        : base(capacity, false) { }

    /// <remarks>This constructor takes ownership of the array. The array must not be modified after
    /// calling this constructor.</remarks>
    public RawBoundedByteList(byte[] bytes)
        : base(bytes, (nuint)bytes.Length, false) { }

    public override void Add(AzothValue value) => AddValue(value.ByteValue);
    public override AzothValue At(nuint index) => AzothValue.Byte(ValueAt(index));
    public override void Set(nuint index, AzothValue value) => SetValue(index, value.ByteValue);

    public string Utf8GetString(nuint start, nuint byteCount)
        => Encoding.UTF8.GetString(AsSpan().Slice((int)start, (int)byteCount));
}
