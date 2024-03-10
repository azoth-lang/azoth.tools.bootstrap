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
        => new(constraint, name, ParameterVariance.Invariant);

    public static GenericParameter Independent(ICapabilityConstraint constraint, StandardTypeName name)
        => new(constraint, name, ParameterVariance.Independent);

    public static GenericParameter Out(ICapabilityConstraint constraint, StandardTypeName name)
        => new(constraint, name, ParameterVariance.Covariant);

    public static GenericParameter In(ICapabilityConstraint constraint, StandardTypeName name)
        => new(constraint, name, ParameterVariance.Contravariant);

    public GenericParameter(ICapabilityConstraint constraint, StandardTypeName name, ParameterVariance parameterVariance)
    {
        Requires.That(nameof(name), name.GenericParameterCount == 0, "Cannot have generic parameters");
        Constraint = constraint;
        ParameterVariance = parameterVariance;
        Name = name;
    }

    public ICapabilityConstraint Constraint { get; }

    public StandardTypeName Name { get; }

    public ParameterVariance ParameterVariance { get; }

    public TypeVariance TypeVariance => ParameterVariance.ToTypeVariance();

    public ParameterIndependence Independence => ParameterVariance.ToParameterIndependence();

    public bool HasIndependence
        => ParameterVariance is ParameterVariance.Independent or ParameterVariance.SharableIndependent;

    // TODO When parameters can be values not just types, add: public DataType DataType { get; }

    #region Equals
    public bool Equals(GenericParameter? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ParameterVariance == other.ParameterVariance && Name.Equals(other.Name);
    }

    public override bool Equals(object? obj)
        => obj is GenericParameter other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(ParameterVariance, Name);

    public static bool operator ==(GenericParameter? left, GenericParameter? right) => Equals(left, right);

    public static bool operator !=(GenericParameter? left, GenericParameter? right) => !Equals(left, right);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        var constraint = Constraint.ToSourceCodeString();
        if (Constraint != CapabilitySet.Aliasable && constraint.Length != 0)
            builder.Append(constraint).Append(' ');
        builder.Append(Name);
        var variance = ParameterVariance.ToSourceCodeString();
        if (variance.Length != 0)
            builder.Append(' ').Append(variance);
        return builder.ToString();
    }
}
