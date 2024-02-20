using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

/// <summary>
/// For function types it is necessary to know not only the type of each parameter, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public readonly record struct Parameter(bool IsLent, DataType Type) : IParameter
{
    public static readonly Parameter Int = new(false, DataType.Int);

    Pseudotype IParameter.Type => Type;

    public bool IsFullyKnown => Type.IsFullyKnown;

    public bool CanOverride(Parameter baseParameter)
        => (!baseParameter.IsLent || IsLent) && Type.IsAssignableFrom(baseParameter.Type);

    public bool CanOverrideSelf(Parameter baseSelfParameter)
        => (!baseSelfParameter.IsLent || IsLent) && baseSelfParameter.Type.IsAssignableFrom(Type);

    public bool ReferenceEquals(Parameter other)
        => IsLent == other.IsLent && ReferenceEquals(Type, other.Type);

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => IsLent ? $"lent {Type.ToILString()}" : Type.ToILString();

    public string ToSourceCodeString()
        => IsLent ? $"lent {Type.ToSourceCodeString()}" : Type.ToSourceCodeString();
}
