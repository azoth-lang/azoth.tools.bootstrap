namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;

internal struct RawBoundedList
{
    public RawBoundedList(nuint capacity)
    {
        Items = new AzothValue[capacity];
        Count = 0;
    }

    public RawBoundedList(nuint count, AzothValue[] items)
    {
        Count = count;
        Items = items;
    }

    public AzothValue[] Items;
    public nuint Count;
}
