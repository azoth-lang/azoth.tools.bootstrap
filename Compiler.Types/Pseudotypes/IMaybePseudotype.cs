using System;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

[Closed(typeof(IPseudotype), typeof(Pseudotype), typeof(IMaybeExpressionType))]
public interface IMaybePseudotype : IEquatable<IMaybePseudotype>
{
    public sealed Pseudotype AsType => (Pseudotype)this;

    /// <summary>
    /// Create a type with the upper bound of the capability constraint.
    /// </summary>
    IMaybeExpressionType ToUpperBound();
}
