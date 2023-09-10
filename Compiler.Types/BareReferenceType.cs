using System;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A reference type without any capability attached
/// </summary>
[Closed(
    typeof(BareObjectType),
    typeof(BareAnyType))]
public abstract class BareReferenceType : IEquatable<BareReferenceType>
{
    public abstract ReferenceType With(ReferenceCapability capability);


    #region MyRegion
    public abstract bool Equals(BareReferenceType? other);

    public override bool Equals(object? obj)
        => obj is BareReferenceType other && Equals(other);

    public abstract override int GetHashCode();

    public static bool operator ==(BareReferenceType? left, BareReferenceType? right) => Equals(left, right);

    public static bool operator !=(BareReferenceType? left, BareReferenceType? right) => !Equals(left, right);
    #endregion

    public abstract override string ToString();
}
