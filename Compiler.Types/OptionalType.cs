using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The type `T?` is an optional type. Optional types either have the value
/// `none` or a value of their referent type. The referent type may be a value
/// type or a reference type. Optional types themselves are always immutable.
/// However ,the referent type may be mutable or immutable. Effectively, optional
/// types are like an immutable struct type `Optional[T]`. However, the value
/// semantics are strange. They depend on the referent type.
/// </summary>
public sealed class OptionalType : NonEmptyType, INonVoidType
{
    public static IMaybeExpressionType Create(IMaybeType referent)
        => referent switch
        {
            IType t => new OptionalType(t),
            UnknownType _ => IType.Unknown,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public static OptionalType Create(IType referent)
        => new OptionalType(referent);

    public IType Referent { get; }

    public override bool AllowsVariance => true;

    public override bool HasIndependentTypeArguments => Referent.HasIndependentTypeArguments;

    public override bool IsFullyKnown => Referent.IsFullyKnown;

    private bool ReferentRequiresParens => Referent is FunctionType or ViewpointType;

    public OptionalType(IType referent)
    {
        if (referent is VoidType)
            throw new ArgumentException("Cannot create `void?` type", nameof(referent));
        Referent = referent;
    }

    public override IMaybeAntetype ToAntetype()
        => Referent.ToAntetype() is INonVoidAntetype referent
            ? new OptionalAntetype(referent) : IAntetype.Unknown;

    public override IType AccessedVia(ICapabilityConstraint capability) => this;

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
