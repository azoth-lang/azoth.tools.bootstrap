namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout.BoundedLists;

internal class RawBoundedList : RawBoundedList<AzothValue>
{
    public RawBoundedList(nuint capacity) : base(capacity, true)
    {
    }

    public override void Add(AzothValue value) => AddValue(value);
    public override AzothValue At(nuint index) => ValueAt(index);
}
