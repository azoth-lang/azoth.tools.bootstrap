using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

// ReSharper disable once InconsistentNaming
public partial interface TypeConstructor
{
    /// <summary>
    /// A generic parameter definition for a type constructor.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public sealed class Parameter : IEquatable<Parameter>
    {
        public static Parameter Invariant(ICapabilityConstraint constraint, IdentifierName name)
            => new(constraint, name, TypeParameterIndependence.None, TypeParameterVariance.Invariant);

        public static Parameter Independent(ICapabilityConstraint constraint, IdentifierName name)
            => new(constraint, name, TypeParameterIndependence.Independent, TypeParameterVariance.Invariant);

        public static Parameter Out(ICapabilityConstraint constraint, IdentifierName name)
            => new(constraint, name, TypeParameterIndependence.None, TypeParameterVariance.Covariant);

        public static Parameter In(ICapabilityConstraint constraint, IdentifierName name)
            => new(constraint, name, TypeParameterIndependence.None, TypeParameterVariance.Contravariant);

        public ICapabilityConstraint Constraint { get; }

        public IdentifierName Name { get; }

        public TypeParameterIndependence Independence { get; }

        public TypeParameterVariance Variance { get; }

        public bool HasIndependence => Independence != TypeParameterIndependence.None;

        // TODO When parameters can be values not just types, add: public DataType DataType { get; }

        public Parameter(
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
        public bool Equals(Parameter? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Constraint.Equals(other.Constraint)
                   && Independence == other.Independence
                   && Variance == other.Variance
                   && Name.Equals(other.Name);
        }

        public override bool Equals(object? obj) => obj is Parameter other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Constraint, Name, Independence, Variance);
        #endregion

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (Constraint != CapabilitySet.Aliasable) builder.Append(Constraint.ToSourceCodeString()).Append(' ');
            builder.Append(Name);
            var independence = Independence.ToSourceCodeString();
            if (independence.Length != 0) builder.Append(' ').Append(independence);
            var variance = Variance.ToSourceCodeString();
            if (variance.Length != 0) builder.Append(' ').Append(variance);
            return builder.ToString();
        }
    }
}
