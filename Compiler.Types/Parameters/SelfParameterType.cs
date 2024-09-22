using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

/// <summary>
/// For function types it is necessary to know not only the type of each parameter, but whether it
/// was declared `lent`. This type packages those to values.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
// TODO IPseudotype still allows for void and const types, which are not valid here.
public record class SelfParameterType(bool IsLent, IPseudotype Type) : IMaybeSelfParameterType
{
    public static IMaybeSelfParameterType Create(bool isLent, IMaybePseudotype type)
    {
        if (type is IPseudotype pseudotype) return new SelfParameterType(isLent, pseudotype);
        return IType.Unknown;
    }

    public static readonly ParameterType Int = new(false, IType.Int);

    IMaybePseudotype IMaybeSelfParameterType.Type => Type;

    public bool CanOverride(SelfParameterType baseParameterType)
        => (!baseParameterType.IsLent || IsLent) && baseParameterType.Type.IsAssignableFrom(Type);

    public ParameterType ToUpperBound() => new(IsLent, (INonVoidType)Type.ToUpperBound().ToNonConstValueType());

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => IsLent ? $"lent {Type.ToILString()}" : Type.ToILString();

    public string ToSourceCodeString()
        => IsLent ? $"lent {Type.ToSourceCodeString()}" : Type.ToSourceCodeString();
}
