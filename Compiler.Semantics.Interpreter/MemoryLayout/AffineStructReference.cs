namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

/// <summary>
/// A reference to an affine struct instance.
/// </summary>
/// <remarks>While affine structs ought to support being stored inline, the interpreter never does
/// that. All affine struct values are stored on the heap and referenced.</remarks>
internal readonly struct AffineStructReference
{
    private readonly Value[] fields;

    public AffineStructReference(AffineStructMetadata metadata)
    {
        fields = metadata.CreateInstanceFields();
    }

    public AffineStructMetadata Metadata => fields[0].AffineStructMetadata;

    public Value this[IFieldDeclarationNode field] => fields[Metadata.GetIndex(field)];
}
