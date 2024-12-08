using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;

[Closed(typeof(IPseudotype), typeof(IMaybeType))]
public interface IMaybePseudotype : IEquatable<IMaybePseudotype>
{
    /// <summary>
    /// Convert this type to the equivalent plainType.
    /// </summary>
    IMaybePlainType ToPlainType();

    /// <summary>
    /// Create a type with the upper bound of the capability constraint.
    /// </summary>
    IMaybeType ToUpperBound();

    /// <summary>
    /// How this type would be written in source code.
    /// </summary>
    string ToSourceCodeString();

    /// <summary>
    /// How this type would be written in IL.
    /// </summary>
    string ToILString();
}
