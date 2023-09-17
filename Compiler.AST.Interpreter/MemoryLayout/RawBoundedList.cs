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

    public void Shrink(nuint count)
    {
        if (count > Count) throw new Abort("Cannot increase Raw_Bounded_List size with `shrink`.");
        var oldCount = Count;
        Count = count;
        // To allow the .NET garbage collector to see collect referenced values, clear that part of
        // the list.
        for (nuint i = count; i < oldCount; i++)
            items[i] = AzothValue.None;
    }
}
