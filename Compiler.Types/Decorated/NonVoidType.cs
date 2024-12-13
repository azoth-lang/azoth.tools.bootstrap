using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[Closed(
    typeof(CapabilitySetSelfType),
    typeof(CapabilityType),
    typeof(FunctionType),
    typeof(GenericParameterType),
    typeof(OptionalType),
    typeof(SelfViewpointType),
    typeof(CapabilityViewpointType),
    typeof(NeverType))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class NonVoidType : IType, IMaybeNonVoidType
{
    public abstract NonVoidPlainType PlainType { get; }
    IPlainType IType.PlainType => PlainType;
    IMaybePlainType IMaybeType.PlainType => PlainType;
    IMaybeNonVoidPlainType IMaybeNonVoidType.PlainType => PlainType;

    public abstract bool HasIndependentTypeArguments { get; }

    public abstract TypeReplacements TypeReplacements { get; }

    public virtual NonVoidType ToNonLiteral() => this;
    IType IType.ToNonLiteral() => ToNonLiteral();

    #region Equality
    public abstract bool Equals(IMaybeType? other);

    public sealed override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is NonVoidType other && Equals(other);

    public abstract override int GetHashCode();

    #endregion

    public sealed override string ToString() => ToILString();

    public abstract string ToSourceCodeString();

    public abstract string ToILString();
}
