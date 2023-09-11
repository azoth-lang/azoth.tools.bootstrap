namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;

internal class RawBoundedList
{
    public RawBoundedList(nuint capacity)
    {
        Count = 0;
        items = new AzothValue[capacity];
    }

    public nuint Count { get; private set; }
    public nuint Capacity => (nuint)items.Length;
    private readonly AzothValue[] items;

    public void Add(AzothValue value)
    {
        if (Count >= Capacity)
            throw new Abort("Cannot add to Raw_Bounded_List.");
        items[Count++] = value;
    }

    public AzothValue At(nuint index) => items[index];
}
