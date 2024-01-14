using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// For function types it is necessary to know not only the type of the return value, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public readonly record struct ReturnType(bool IsLent, DataType Type)
{
    public static readonly ReturnType Void = new(false, DataType.Void);
    public static readonly ReturnType Size = new(false, DataType.Size);
    public static readonly ReturnType Never = new(false, DataType.Never);
    public static readonly ReturnType Int = new(false, DataType.Int);
    public static readonly ReturnType UInt64 = new(false, DataType.UInt64);

    public bool CanOverride(ReturnType baseReturnType)
        => (!IsLent || baseReturnType.IsLent) && baseReturnType.Type.IsAssignableFrom(Type);

    public string ToILString() => IsLent ? $"lent {Type.ToILString()}" : Type.ToILString();

    public string ToSourceCodeString()
        => IsLent ? $"lent {Type.ToSourceCodeString()}" : Type.ToSourceCodeString();
}
