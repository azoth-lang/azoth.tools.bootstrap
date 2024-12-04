using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;

/// <summary>
/// For function types it is necessary to know not only the type of each parameter, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public record class ParameterType(bool IsLent, INonVoidType Type) : IMaybeParameterType
{
    public static IMaybeParameterType Create(bool isLent, IMaybeNonVoidType type)
    {
        if (type is INonVoidType t)
            return new ParameterType(isLent, t);
        return IType.Unknown;
    }

    public static readonly ParameterType Int = new(false, IType.Int);

    IMaybeNonVoidType IMaybeParameterType.Type => Type;

    public bool CanOverride(ParameterType baseParameter)
        => (!baseParameter.IsLent || IsLent) && Type.IsAssignableFrom(baseParameter.Type);

    public bool CanOverrideSelf(ParameterType baseSelfParameter)
        => (!baseSelfParameter.IsLent || IsLent) && baseSelfParameter.Type.IsAssignableFrom(Type);

    public bool ReferenceEquals(ParameterType other)
        => IsLent == other.IsLent && ReferenceEquals(Type, other.Type);

    public IMaybeNonVoidPlainType ToPlainType() => Type.ToPlainType();

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => IsLent ? $"lent {Type.ToILString()}" : Type.ToILString();

    public string ToSourceCodeString()
        => IsLent ? $"lent {Type.ToSourceCodeString()}" : Type.ToSourceCodeString();
}
