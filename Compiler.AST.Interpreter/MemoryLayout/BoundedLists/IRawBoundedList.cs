namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout.BoundedLists;

internal interface IRawBoundedList
{
    public nuint Count { get; }
    public nuint Capacity { get; }
    public void Add(AzothValue value);
    public AzothValue At(nuint index);
    public void Set(nuint index, AzothValue value);
    public void Shrink(nuint count);
}
