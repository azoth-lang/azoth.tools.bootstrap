using System;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The type `T?` is an optional type. Optional types either have the value
/// `none` or a value of their referent type. The referent type may be a value
/// type or a reference type. Optional types themselves are always immutable.
/// However the referent type may be mutable or immutable. Effectively, optional
/// types are like an immutable struct type `Optional[T]`. However, the value
/// semantics are strange. They depend on the referent type.
/// </summary>
public sealed class OptionalType : NonEmptyType
{
    public DataType Referent { get; }

    public override bool AllowsVariance => true;

    public override bool HasIndependentTypeArguments => Referent.HasIndependentTypeArguments;

    public override bool IsFullyKnown => Referent.IsFullyKnown;

    private bool ReferentRequiresParens => Referent is FunctionType or ViewpointType;

    public OptionalType(DataType referent)
    {
        if (referent is VoidType)
            throw new ArgumentException("Cannot create `void?` type", nameof(referent));
        Referent = referent;
    }

    #region Equals
    public override bool Equals(DataType? other)
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
