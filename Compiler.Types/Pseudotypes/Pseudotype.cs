using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

[Closed(typeof(DataType), typeof(CapabilityTypeConstraint))]
public abstract class Pseudotype : IMaybePseudotype
{
    /// <summary>
    /// A known type is one that has no unknown parts.
    /// </summary>
    public abstract bool IsFullyKnown { get; }

    /// <summary>
    /// Create a type with the upper bound of the capability constraint.
    /// </summary>
    public abstract IMaybeExpressionType ToUpperBound();

    /// <summary>
    /// Convert this type to the equivalent antetype.
    /// </summary>
    public abstract IMaybeExpressionAntetype ToAntetype();

    #region Equality
    public abstract bool Equals(IMaybePseudotype? other);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is IMaybePseudotype other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public sealed override string ToString()
        => throw new NotSupportedException();

    /// <summary>
    /// How this type would be written in source code.
    /// </summary>
    public abstract string ToSourceCodeString();

    /// <summary>
    /// How this type would be written in IL.
    /// </summary>
    public abstract string ToILString();
}
