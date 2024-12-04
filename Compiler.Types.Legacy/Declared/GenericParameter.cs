using System;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

/// <summary>
/// A generic parameter to a type.
/// </summary>
/// <remarks>At the moment all generic parameters are types. In the future, they don't have to be.
/// That is why this class exists to ease refactoring later.</remarks>
public sealed class GenericParameter : IEquatable<GenericParameter>
{
    public static GenericParameter Invariant(ICapabilityConstraint constraint, IdentifierName name)
        => new(constraint, name, TypeParameterIndependence.None, TypeParameterVariance.Invariant);

    public static GenericParameter Independent(ICapabilityConstraint constraint, IdentifierName name)
        => new(constraint, name, TypeParameterIndependence.Independent, TypeParameterVariance.Invariant);

    public static GenericParameter Out(ICapabilityConstraint constraint, IdentifierName name)
        => new(constraint, name, TypeParameterIndependence.None, TypeParameterVariance.Covariant);

    public static GenericParameter In(ICapabilityConstraint constraint, IdentifierName name)
        => new(constraint, name, TypeParameterIndependence.None, TypeParameterVariance.Contravariant);

    public GenericParameter(ICapabilityConstraint constraint, IdentifierName name, TypeParameterIndependence independence, TypeParameterVariance variance)
    {
        Requires.That(name.GenericParameterCount == 0, nameof(name), "Cannot have generic parameters");
        Constraint = constraint;
        Independence = independence;
        Variance = variance;
        Name = name;
    }

    public ICapabilityConstraint Constraint { get; }

    public IdentifierName Name { get; }

    public TypeParameterIndependence Independence { get; }

    public TypeParameterVariance Variance { get; }

    public bool HasIndependence => Independence != TypeParameterIndependence.None;

    // TODO When parameters can be values not just types, add: public DataType DataType { get; }

    #region Equals
    public bool Equals(GenericParameter? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Independence == other.Independence
               && Variance == other.Variance
               && Name.Equals(other.Name);
    }

    public override bool Equals(object? obj)
        => obj is GenericParameter other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Name, Independence, Variance);

    public static bool operator ==(GenericParameter? left, GenericParameter? right)
        => Equals(left, right);

    public static bool operator !=(GenericParameter? left, GenericParameter? right)
        => !Equals(left, right);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        var constraint = Constraint.ToSourceCodeString();
        if (Constraint != CapabilitySet.Aliasable)
            builder.Append(constraint).Append(' ');
        builder.Append(Name);
        var independence = Independence.ToSourceCodeString();
        if (independence.Length != 0)
            builder.Append(' ').Append(independence);
        var variance = Variance.ToSourceCodeString();
        if (variance.Length != 0)
            builder.Append(' ').Append(variance);
        return builder.ToString();
    }
}
