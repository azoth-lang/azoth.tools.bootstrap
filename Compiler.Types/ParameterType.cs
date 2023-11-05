using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// For function types it is necessary to know not only the type of each parameter, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public readonly record struct ParameterType(bool IsLentBinding, DataType Type)
{
    public static readonly ParameterType Int = new(false, DataType.Int);

    public bool CanOverride(ParameterType baseParameterType)
        => (!baseParameterType.IsLentBinding || IsLentBinding) && Type.IsAssignableFrom(baseParameterType.Type);

    public bool CanOverrideSelf(ParameterType baseSelfParameterType)
        => (!baseSelfParameterType.IsLentBinding || IsLentBinding) && baseSelfParameterType.Type.IsAssignableFrom(Type);

    public string ToILString() => IsLentBinding ? $"lent {Type.ToILString()}" : Type.ToILString();

    public string ToSourceCodeString()
        => IsLentBinding ? $"lent {Type.ToSourceCodeString()}" : Type.ToSourceCodeString();
}
