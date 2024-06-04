using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

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
    /// <param name="nonwritableSelf">Whether the self parameter type is nonwriteable.
    /// <see langword="null"/> is used for base types to indicate that it could behave either way.</param>
    public static bool CompatibleWith(this Variance contextVariance, ParameterVariance parameterVariance, bool? nonwritableSelf)
    {
        // if NonwritableCovariant could behave either way, then the most restrictive is used
        var variance = parameterVariance.ToVariance(nonwritableSelf) ?? Variance.Covariant;
        return contextVariance switch
        {
            Variance.Contravariant => variance <= Variance.Invariant,
            Variance.Invariant => variance == Variance.Invariant,
            Variance.Covariant => variance >= Variance.Invariant,
            _ => throw ExhaustiveMatch.Failed(contextVariance),
        };
    }

    public static Variance Inverse(this Variance variance)
        => (Variance)(-(int)variance);
}
