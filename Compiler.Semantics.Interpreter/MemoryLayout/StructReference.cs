namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

/// <summary>
/// A reference to a struct instance.
/// </summary>
/// <remarks>While structs ought to support being stored inline, the interpreter never does that.
/// All struct values are stored on the heap and referenced.</remarks>
internal readonly struct StructReference
{
    private readonly Value[] fields;

    public StructReference(StructMetadata metadata)
    {
        fields = metadata.CreateInstanceFields();
    }

    public StructMetadata Metadata => fields[0].StructMetadata;

    public Value this[IFieldDeclarationNode field] => fields[Metadata.GetIndex(field)];
}
