using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

/// <summary>
/// For function types it is necessary to know not only the type of each parameter, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public readonly record struct ParameterType(bool IsLent, DataType Type) : IParameterType
{
    public static readonly ParameterType Int = new(false, DataType.Int);

    Pseudotype IParameterType.Type => Type;

    public bool IsFullyKnown => Type.IsFullyKnown;

    public bool CanOverride(ParameterType baseParameterType)
        => (!baseParameterType.IsLent || IsLent) && Type.IsAssignableFrom(baseParameterType.Type);

    public bool CanOverrideSelf(ParameterType baseSelfParameterType)
        => (!baseSelfParameterType.IsLent || IsLent) && baseSelfParameterType.Type.IsAssignableFrom(Type);

    public bool ReferenceEquals(ParameterType other)
        => IsLent == other.IsLent && ReferenceEquals(Type, other.Type);

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => IsLent ? $"lent {Type.ToILString()}" : Type.ToILString();

    public string ToSourceCodeString()
        => IsLent ? $"lent {Type.ToSourceCodeString()}" : Type.ToSourceCodeString();
}
