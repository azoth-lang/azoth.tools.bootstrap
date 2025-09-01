namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

/// <summary>
/// A reference to a value instance.
/// </summary>
/// <remarks>While values ought to support being stored inline, the interpreter never does that.
/// All values are stored on the heap and referenced.</remarks>
internal readonly struct ValueReference
{
    private readonly Value[] fields;

    public ValueReference(ValueMetadata metadata)
    {
        fields = metadata.CreateInstanceFields();
    }

    public ValueMetadata Metadata => fields[0].ValueMetadata;

    public Value this[IFieldDeclarationNode field] => fields[Metadata.GetIndex(field)];
}
