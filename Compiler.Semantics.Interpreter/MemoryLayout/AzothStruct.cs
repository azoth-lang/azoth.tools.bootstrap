namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal readonly struct AzothStruct
{
    private readonly AzothValue[] fields;

    public AzothStruct(StructLayout layout)
    {
        fields = layout.CreateInstanceFields();
    }

    public StructLayout Layout => fields[0].StructLayoutValue;

    public AzothValue this[IFieldDeclarationNode field] => fields[Layout.GetIndex(field)];
}
