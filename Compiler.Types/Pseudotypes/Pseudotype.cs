using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

[Closed(typeof(DataType), typeof(CapabilityTypeConstraint))]
public abstract class Pseudotype
{
    /// <summary>
    /// A known type is one that has no unknown parts.
    /// </summary>
    public abstract bool IsFullyKnown { get; }

    /// <summary>
    /// Create a type with the upper bound of the capability constraint.
    /// </summary>
    public abstract DataType ToUpperBound();

    /// <summary>
    /// Convert this type to the equivalent antetype.
    /// </summary>
    public abstract IMaybeExpressionAntetype ToAntetype();

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
