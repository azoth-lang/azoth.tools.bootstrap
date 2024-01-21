using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// For function types it is necessary to know not only the type of each parameter, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public readonly record struct ParameterType(bool IsLent, DataType Type)
{
    public static readonly ParameterType Int = new(false, DataType.Int);

    public bool IsFullyKnown => Type.IsFullyKnown;

    public bool CanOverride(ParameterType baseParameterType)
        => (!baseParameterType.IsLent || IsLent) && Type.IsAssignableFrom(baseParameterType.Type);

    public bool CanOverrideSelf(ParameterType baseSelfParameterType)
        => (!baseSelfParameterType.IsLent || IsLent) && baseSelfParameterType.Type.IsAssignableFrom(Type);

    public string ToILString() => IsLent ? $"lent {Type.ToILString()}" : Type.ToILString();

    public string ToSourceCodeString()
        => IsLent ? $"lent {Type.ToSourceCodeString()}" : Type.ToSourceCodeString();
}
