using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;

[Closed(typeof(IPseudotype), typeof(IMaybeExpressionType))]
public interface IMaybePseudotype : IEquatable<IMaybePseudotype>
{
    /// <summary>
    /// Convert this type to the equivalent antetype.
    /// </summary>
    IMaybeAntetype ToAntetype();

    /// <summary>
    /// Create a type with the upper bound of the capability constraint.
    /// </summary>
    IMaybeExpressionType ToUpperBound();

    /// <summary>
    /// How this type would be written in source code.
    /// </summary>
    string ToSourceCodeString();

    /// <summary>
    /// How this type would be written in IL.
    /// </summary>
    string ToILString();
}
