namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout.BoundedLists;

internal interface IRawBoundedList : IIntrinsicValue
{
    public AzothValue Fixed { get; set; }
    public nuint Count { get; }
    public nuint Capacity { get; }
    public void Add(AzothValue value);
    public AzothRef RefAt(nuint index);
    public AzothValue At(nuint index);
    public void Set(nuint index, AzothValue value);
    public void Shrink(nuint count);
}
