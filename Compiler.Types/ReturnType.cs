namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// For function types it is necessary to know not only the type of the return value, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
public readonly record struct ReturnType(bool IsLent, DataType Type)
{
    public static readonly ReturnType Void = new(false, DataType.Void);
    public static readonly ReturnType Size = new(false, DataType.Size);
    public static readonly ReturnType Never = new(false, DataType.Never);
    public static readonly ReturnType Int = new(false, DataType.Int);

    public string ToILString() => IsLent ? $"lent {Type.ToILString()}" : Type.ToILString();
}
