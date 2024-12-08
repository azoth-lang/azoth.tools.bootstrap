using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

/// <summary>
/// A type that at least in theory can be embodied by values.
/// </summary>
[Closed(
    typeof(CapabilityType),
    typeof(GenericParameterType),
    typeof(ViewpointType),
    typeof(FunctionType),
    typeof(OptionalType))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
// TODO move ConstValueType out of this class since it makes this really a NonEmptyExpressionType
public abstract class NonEmptyType : INonVoidType
{
    private protected NonEmptyType() { }
    public virtual bool AllowsVariance => false;
    public virtual bool HasIndependentTypeArguments => false;

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public virtual IType ReplaceTypeParametersIn(IType type) => type;

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public virtual IMaybeType ReplaceTypeParametersIn(IMaybeType type) => type;

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public virtual IPseudotype ReplaceTypeParametersIn(IPseudotype pseudotype) => pseudotype;

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public virtual IMaybePseudotype ReplaceTypeParametersIn(IMaybePseudotype pseudotype) => pseudotype;

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public ParameterType? ReplaceTypeParametersIn(ParameterType type)
    {
        if (ReplaceTypeParametersIn(type.Type) is not INonVoidType nonVoidType)
            return null;
        return type with { Type = nonVoidType };
    }

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public IMaybeParameterType? ReplaceTypeParametersIn(IMaybeParameterType type)
    {
        if (type is ParameterType parameterType)
            return ReplaceTypeParametersIn(parameterType);
        return IType.Unknown;
    }

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public SelfParameterType ReplaceTypeParametersIn(SelfParameterType type)
        => type with { Type = ReplaceTypeParametersIn(type.Type) };

    /// <summary>
    /// Replace any <see cref="GenericParameterType"/> from this type that appear in the given type
    /// with the type arguments from this type (assuming it has them).
    /// </summary>
    /// <remarks>Has no effect if this is not a generic type.</remarks>
    public IMaybeSelfParameterType ReplaceTypeParametersIn(IMaybeSelfParameterType type)
    {
        if (type is SelfParameterType selfParameterType)
            return ReplaceTypeParametersIn(selfParameterType);
        return IType.Unknown;
    }

    public IType ToUpperBound() => this;

    /// <summary>
    /// Convert this type to the equivalent plainType.
    /// </summary>
    public abstract INonVoidPlainType ToPlainType();

    /// <summary>
    /// Convert types for constant values to their corresponding types.
    /// </summary>
    public virtual IType ToNonConstValueType() => this;

    /// <summary>
    /// The same type except with any mutability removed.
    /// </summary>
    public virtual IMaybeNonVoidType WithoutWrite() => this;

    /// <summary>
    /// Return the type for when a value of this type is accessed via a type of the given value.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public IMaybeType AccessedVia(IMaybePseudotype contextType)
    {
        if (contextType is CapabilityType capabilityType)
            return AccessedVia(capabilityType.Capability);
        if (contextType is CapabilityTypeConstraint capabilityTypeConstraint)
            return AccessedVia(capabilityTypeConstraint.Capability);
        return this;
    }

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public virtual IType AccessedVia(ICapabilityConstraint capability) => this;

    public abstract Decorated.INonVoidType ToDecoratedType();

    IMaybeType IMaybeType.AccessedVia(ICapabilityConstraint capability)
        => AccessedVia(capability);

    #region Equality
    public abstract bool Equals(IMaybeType? other);

    public abstract override int GetHashCode();

    public bool Equals(IMaybePseudotype? other)
        => ReferenceEquals(this, other) || other is IMaybeType type && Equals(type);

    public sealed override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is IMaybeType type && Equals(type);
    #endregion

    public sealed override string ToString() => throw new NotSupportedException();

    /// <summary>
    /// How this type would be written in source code.
    /// </summary>
    public abstract string ToSourceCodeString();

    /// <summary>
    /// How this type would be written in IL.
    /// </summary>
    public abstract string ToILString();
}
