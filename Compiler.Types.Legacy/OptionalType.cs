using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

/// <summary>
/// The type `T?` is an optional type. Optional types either have the value
/// `none` or a value of their referent type. The referent type may be a value
/// type or a reference type. Optional types themselves are always immutable.
/// However, the referent type may be mutable or immutable. Effectively, optional
/// types are like an immutable struct type `Optional[T]`. However, the value
/// semantics are strange. They depend on the referent type.
/// </summary>
public sealed class OptionalType : NonEmptyType, INonVoidType
{
    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>Unknown and void types are not changed.</remarks>
    public static IMaybeType Create(IMaybeType referent)
        => referent switch
        {
            INonVoidType t => new OptionalType(t),
            UnknownType _ => IType.Unknown,
            VoidType _ => IType.Void,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>Unknown type produces unknown type.</remarks>
    public static IMaybeNonVoidType Create(IMaybeNonVoidType referent)
        => referent switch
        {
            INonVoidType t => new OptionalType(t),
            UnknownType _ => IType.Unknown,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public static OptionalType Create(INonVoidType referent)
        => new OptionalType(referent);

    public INonVoidType Referent { get; }

    public override bool AllowsVariance => true;

    public override bool HasIndependentTypeArguments => Referent.HasIndependentTypeArguments;

    private bool ReferentRequiresParens => Referent is FunctionType or ViewpointType;

    IMaybeNonVoidType IMaybeNonVoidType.WithoutWrite() => this;

    public OptionalType(INonVoidType referent)
    {
        Referent = referent;
    }

    public override INonVoidPlainType ToPlainType()
        => new OptionalPlainType(Referent.ToPlainType());

    IMaybeType IMaybeType.AccessedVia(IMaybePseudotype contextType) => (IMaybeType)AccessedVia(contextType);

    public override IType AccessedVia(ICapabilityConstraint capability) => this;
    IMaybeType IMaybeType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);

    #region Equals
    public override bool Equals(IMaybeExpressionType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OptionalType otherType
               && Equals(Referent, otherType.Referent);
    }

    public override int GetHashCode()
        // Use the type to give a different hashcode than just the referent
        => HashCode.Combine(typeof(OptionalType), Referent);
    #endregion

    public override string ToSourceCodeString()
        => ReferentRequiresParens ? $"({Referent.ToSourceCodeString()})?" : $"{Referent.ToSourceCodeString()}?";

    public override string ToILString()
        => ReferentRequiresParens ? $"({Referent.ToILString()})?" : $"{Referent.ToILString()}?";
}
