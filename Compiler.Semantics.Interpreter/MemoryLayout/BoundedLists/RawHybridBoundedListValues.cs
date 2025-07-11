namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout.BoundedLists;

internal sealed class RawHybridBoundedListValues : RawHybridBoundedListBase<AzothValue>
{
    public RawHybridBoundedListValues(nuint capacity) : base(capacity, true) { }

    public override void Add(AzothValue value) => AddValue(value);
    public override AzothValue At(nuint index) => ValueAt(index);
    public override void Set(nuint index, AzothValue value) => SetValue(index, value);
}
