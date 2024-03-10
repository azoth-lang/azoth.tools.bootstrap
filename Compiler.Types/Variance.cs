using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public enum Variance
{
    Contravariant = -1,
    Invariant = 0,
    Covariant = 1,
}

public static class VarianceExtensions
{
    /// <summary>
    /// Is the given variance compatible with this one?
    /// </summary>
    internal static bool CompatibleWith(this Variance contextVariance, ParameterVariance parameterVariance, bool nonwritableSelf)
    {
        var variance = parameterVariance.ToVariance(nonwritableSelf);
        return contextVariance switch
        {
            Variance.Contravariant => variance <= Variance.Invariant,
            Variance.Invariant => variance == Variance.Invariant,
            Variance.Covariant => variance >= Variance.Invariant,
            _ => throw ExhaustiveMatch.Failed(contextVariance),
        };
    }

    internal static Variance Inverse(this Variance variance)
        => (Variance)(-(int)variance);
}
