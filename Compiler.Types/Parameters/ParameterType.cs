using System;
using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

/// <summary>
/// For function types it is necessary to know not only the type of each parameter, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public record class ParameterType(bool IsLent, IType Type) : IMaybeParameterType
{
    public static IMaybeParameterType Create(bool isLent, IMaybeType type)
    {
        if (type is IType t)
            return new ParameterType(isLent, t);
        return IType.Unknown;
    }

    public static readonly ParameterType Int = new(false, IType.Int);

    IMaybeType IMaybeParameterType.Type => Type;

    public bool IsFullyKnown => Type.IsFullyKnown;

    public bool CanOverride(ParameterType baseParameter)
        => (!baseParameter.IsLent || IsLent) && Type.IsAssignableFrom(baseParameter.Type);

    public bool CanOverrideSelf(ParameterType baseSelfParameter)
        => (!baseSelfParameter.IsLent || IsLent) && baseSelfParameter.Type.IsAssignableFrom(Type);

    public bool ReferenceEquals(ParameterType other)
        => IsLent == other.IsLent && ReferenceEquals(Type, other.Type);

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => IsLent ? $"lent {Type.ToILString()}" : Type.ToILString();

    public string ToSourceCodeString()
        => IsLent ? $"lent {Type.ToSourceCodeString()}" : Type.ToSourceCodeString();
}
