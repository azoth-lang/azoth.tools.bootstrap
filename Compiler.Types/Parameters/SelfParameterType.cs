using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

/// <summary>
/// For function types it is necessary to know not only the type of each parameter, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public readonly record struct SelfParameterType(bool IsLent, IMaybePseudotype Type) : IParameterType
{
    public static readonly ParameterType Int = new(false, IType.Int);

    public bool IsFullyKnown => Type.IsFullyKnown;

    public bool CanOverride(SelfParameterType baseParameterType)
        => (!baseParameterType.IsLent || IsLent) && baseParameterType.Type.IsAssignableFrom(Type);

    public ParameterType ToUpperBound() => new(IsLent, Type.ToUpperBound());

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => IsLent ? $"lent {Type.ToILString()}" : Type.ToILString();

    public string ToSourceCodeString()
        => IsLent ? $"lent {Type.ToSourceCodeString()}" : Type.ToSourceCodeString();
}
