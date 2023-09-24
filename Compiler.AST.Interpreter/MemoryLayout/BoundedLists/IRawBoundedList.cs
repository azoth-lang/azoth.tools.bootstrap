namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout.BoundedLists;

internal interface IRawBoundedList
{
    public nuint Count { get; }
    public nuint Capacity { get; }
    void Add(AzothValue value);
    public AzothValue At(nuint index);
    void Shrink(nuint count);
}
