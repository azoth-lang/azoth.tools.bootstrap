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
public readonly record struct Return(DataType Type)
{
    public static readonly Return Void = new(DataType.Void);
    public static readonly Return Size = new(DataType.Size);
    public static readonly Return Never = new(DataType.Never);
    public static readonly Return Int = new(DataType.Int);
    public static readonly Return UInt64 = new(DataType.UInt64);

    public bool IsFullyKnown => Type.IsFullyKnown;

    public bool CanOverride(Return baseReturn)
        => baseReturn.Type.IsAssignableFrom(Type);

    public bool ReferenceEquals(Return other)
        => ReferenceEquals(Type, other.Type);

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => Type.ToILString();

    public string ToSourceCodeString()
        => Type.ToSourceCodeString();
}
