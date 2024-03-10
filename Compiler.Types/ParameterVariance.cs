using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public enum ParameterVariance
{
    Contravariant = -1,
    Invariant = 0,
    NonwritableCovariant = 1,
    Covariant = 2,
}

public static class TypeVarianceExtensions
{
    public static string ToSourceCodeString(this ParameterVariance variance)
        => variance switch
        {
            ParameterVariance.Contravariant => "in",
            ParameterVariance.Invariant => "",
            ParameterVariance.NonwritableCovariant => "nonwriteable out",
            ParameterVariance.Covariant => "out",
            _ => throw ExhaustiveMatch.Failed(variance),
        };

    /// <summary>
    /// Is the given variance compatible with this one?
    /// </summary>
    internal static bool CompatibleWith(this ParameterVariance contextVariance, ParameterVariance variance, bool nonwritableSelf)
    {
        // NonwritableCovariant acts like Covariant or Invariant depending on whether self is nonwritable
        if (variance == ParameterVariance.NonwritableCovariant)
            variance = nonwritableSelf ? ParameterVariance.Covariant : ParameterVariance.Invariant;
        return contextVariance switch
        {
            ParameterVariance.Contravariant => variance is ParameterVariance.Contravariant or ParameterVariance.Invariant,
            ParameterVariance.Invariant => variance is ParameterVariance.Invariant,
            ParameterVariance.NonwritableCovariant => variance is ParameterVariance.Invariant,
            ParameterVariance.Covariant => variance is ParameterVariance.Covariant or ParameterVariance.Invariant,
            _ => throw ExhaustiveMatch.Failed(contextVariance),
        };
    }

    internal static ParameterVariance Inverse(this ParameterVariance variance)
        => variance switch
        {
            ParameterVariance.Contravariant => ParameterVariance.Covariant,
            ParameterVariance.Invariant => ParameterVariance.Invariant,
            ParameterVariance.NonwritableCovariant => ParameterVariance.Invariant,
            ParameterVariance.Covariant => ParameterVariance.Contravariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
}
