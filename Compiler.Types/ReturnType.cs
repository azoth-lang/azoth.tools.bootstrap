using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// This was created to mirror <see cref="Parameter"/> when return types needed `lent` support
/// the same as parameters. `lent` return has been removed, but this is being left around for now
/// in case it is useful in the future. If it is not, it will be removed.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public readonly record struct ReturnType(DataType Type)
{
    public static readonly ReturnType Void = new(DataType.Void);
    public static readonly ReturnType Size = new(DataType.Size);
    public static readonly ReturnType Never = new(DataType.Never);
    public static readonly ReturnType Int = new(DataType.Int);
    public static readonly ReturnType UInt64 = new(DataType.UInt64);

    public bool IsFullyKnown => Type.IsFullyKnown;

    public bool CanOverride(ReturnType baseReturnType)
        => baseReturnType.Type.IsAssignableFrom(Type);

    public bool ReferenceEquals(ReturnType other)
        => ReferenceEquals(Type, other.Type);

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => Type.ToILString();

    public string ToSourceCodeString()
        => Type.ToSourceCodeString();
}
