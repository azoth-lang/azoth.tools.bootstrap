using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// A parameter definition for a type constructor.
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class TypeConstructorParameter : IEquatable<TypeConstructorParameter>
{
    public static TypeConstructorParameter Invariant(ICapabilityConstraint constraint, IdentifierName name)
        => new(constraint, name, TypeParameterIndependence.None, TypeParameterVariance.Invariant);

    public static TypeConstructorParameter Independent(ICapabilityConstraint constraint, IdentifierName name)
        => new(constraint, name, TypeParameterIndependence.Independent, TypeParameterVariance.Invariant);

    public static TypeConstructorParameter ShareableIndependent(ICapabilityConstraint constraint, IdentifierName name)
        => new(constraint, name, TypeParameterIndependence.ShareableIndependent, TypeParameterVariance.Invariant);

    public static TypeConstructorParameter IndependentReadOnlyOut(ICapabilityConstraint constraint, IdentifierName name)
        => new(constraint, name, TypeParameterIndependence.Independent, TypeParameterVariance.ReadOnlyCovariant);

    public static TypeConstructorParameter Out(ICapabilityConstraint constraint, IdentifierName name)
        => new(constraint, name, TypeParameterIndependence.None, TypeParameterVariance.Covariant);

    public static TypeConstructorParameter In(ICapabilityConstraint constraint, IdentifierName name)
        => new(constraint, name, TypeParameterIndependence.None, TypeParameterVariance.Contravariant);

    public ICapabilityConstraint Constraint { [DebuggerStepThrough] get; }

    public IdentifierName Name { [DebuggerStepThrough] get; }

    public TypeParameterIndependence Independence { [DebuggerStepThrough] get; }

    public TypeParameterVariance Variance { [DebuggerStepThrough] get; }

    public bool HasIndependence => Independence != TypeParameterIndependence.None;

    // TODO When parameters can be values not just types, add: public DataType DataType { get; }

    public TypeConstructorParameter(
        ICapabilityConstraint constraint,
        IdentifierName name,
        TypeParameterIndependence independence,
        TypeParameterVariance variance)
    {
        Constraint = constraint;
        Name = name;
        Independence = independence;
        Variance = variance;
    }

    #region Equality
    public bool Equals(TypeConstructorParameter? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Constraint.Equals(other.Constraint)
               && Name.Equals(other.Name)
               && Independence == other.Independence
               && Variance == other.Variance;
    }

    public override bool Equals(object? obj) => obj is TypeConstructorParameter other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Constraint, Name, Independence, Variance);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        if (Independence != TypeParameterIndependence.None)
        {
            // Independent parameters are always `any` so that isn't written
            builder.Append(Independence.ToSourceCodeString()).Append(' ');
        }
        else
        {
            // Aliasable is the default and so isn't written
            if (Constraint != CapabilitySet.Aliasable) builder.Append(Constraint.ToSourceCodeString()).Append(' ');
        }
        builder.Append(Name);
        var variance = Variance.ToSourceCodeString();
        if (variance.Length != 0) builder.Append(' ').Append(variance);
        return builder.ToString();
    }
}
