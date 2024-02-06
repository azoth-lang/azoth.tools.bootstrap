using System;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

[Closed(typeof(DataType), typeof(ObjectTypeConstraint))]
public abstract class Pseudotype
{
    /// <summary>
    /// A known type is one that has no unknown parts.
    /// </summary>
    public abstract bool IsFullyKnown { get; }

    public abstract DataType ToUpperBound();

    [Obsolete("Use ToSourceCodeString() or ToILString() instead", error: true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public sealed override string ToString()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
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
