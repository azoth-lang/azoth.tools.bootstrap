namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout.BoundedLists;

internal class RawBoundedByteList : RawBoundedList<byte>
{
    public RawBoundedByteList(nuint capacity)
        : base(capacity, false) { }

    public override void Add(AzothValue value) => AddValue(value.ByteValue);

    public override AzothValue At(nuint index) => AzothValue.Byte(ValueAt(index));
}
