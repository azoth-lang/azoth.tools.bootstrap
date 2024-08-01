namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout.Experiment;

internal class AzothObject<TReferences, TInts, TBytes> : IAzothObject
    where TReferences : struct, IAzothReferences
    where TInts : struct, IAzothInts
    where TBytes : struct, IAzothBytes
{
    public VTable VTable { get; }

    private TReferences references;
    private TInts ints;
    private TBytes bytes;

    public AzothObject(VTable vTable)
    {
        VTable = vTable;
    }

    public void Foo()
    {
        _ = references.AsSpan();
        _ = ints.AsSpan();
        _ = bytes.AsSpan();
    }
}
