using System;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

/// <summary>
/// A generic parameter to a type.
/// </summary>
/// <remarks>At the moment all generic parameters are types. In the future, they don't have to be.
/// That is why this class exists to ease refactoring later.</remarks>
public sealed class GenericParameter : IEquatable<GenericParameter>
{
    public static GenericParameter Invariant(ICapabilityConstraint constraint, StandardTypeName name)
        => new(constraint, name, ParameterIndependence.None, ParameterVariance.Invariant);

    public static GenericParameter Independent(ICapabilityConstraint constraint, StandardTypeName name)
        => new(constraint, name, ParameterIndependence.Independent, ParameterVariance.Invariant);

    public static GenericParameter Out(ICapabilityConstraint constraint, StandardTypeName name)
        => new(constraint, name, ParameterIndependence.None, ParameterVariance.Covariant);

    public static GenericParameter In(ICapabilityConstraint constraint, StandardTypeName name)
        => new(constraint, name, ParameterIndependence.None, ParameterVariance.Contravariant);

    public GenericParameter(ICapabilityConstraint constraint, StandardTypeName name, ParameterIndependence independence, ParameterVariance variance)
    {
        Requires.That(nameof(name), name.GenericParameterCount == 0, "Cannot have generic parameters");
        Constraint = constraint;
        Independence = independence;
        Variance = variance;
        Name = name;
    }

    public ICapabilityConstraint Constraint { get; }

    public StandardTypeName Name { get; }

    public ParameterIndependence Independence { get; }

    public ParameterVariance Variance { get; }

    public bool HasIndependence => Independence != ParameterIndependence.None;

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
