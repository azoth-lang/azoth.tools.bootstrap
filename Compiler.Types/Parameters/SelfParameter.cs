using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

/// <summary>
/// For function types it is necessary to know not only the type of each parameter, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public readonly record struct SelfParameter(bool IsLent, Pseudotype Type) : IParameter
{
    public static readonly Parameter Int = new(false, DataType.Int);

    public bool IsFullyKnown => Type.IsFullyKnown;

    public bool CanOverride(SelfParameter baseParameterType)
        => (!baseParameterType.IsLent || IsLent) && baseParameterType.Type.IsAssignableFrom(Type);

    public Parameter ToUpperBound() => new(IsLent, Type.ToUpperBound());

    public bool ReferenceEquals(Parameter other)
        => IsLent == other.IsLent && ReferenceEquals(Type, other.Type);

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => IsLent ? $"lent {Type.ToILString()}" : Type.ToILString();

    public string ToSourceCodeString()
        => IsLent ? $"lent {Type.ToSourceCodeString()}" : Type.ToSourceCodeString();
}
