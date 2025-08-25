namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal readonly struct AzothValueType
{
    private readonly AzothValue[] fields;

    public AzothValueType(ValueLayout layout)
    {
        fields = layout.CreateInstanceFields();
    }

    public StructLayout Layout => fields[0].StructLayoutValue;

    public AzothValue this[IFieldDeclarationNode field] => fields[Layout.GetIndex(field)];
}
