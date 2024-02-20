using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public enum ParameterVariance
{
    Contravariant = -1,
    Invariant = 0,
    Independent = 1,
    Covariant = 2,
}

public static class ParameterVarianceExtensions
{
    public static string ToSourceCodeString(this ParameterVariance variance) =>
        variance switch
        {
            ParameterVariance.Contravariant => "in",
            ParameterVariance.Invariant => "",
            ParameterVariance.Independent => "ind",
            ParameterVariance.Covariant => "out",
            _ => throw ExhaustiveMatch.Failed(variance),
        };

    internal static bool CompatibleWith(this ParameterVariance variance, ParameterVariance other)
        => variance switch
        {
            // TODO is this correct that Independent is compatible with Contravariant (just like Invariant)?
            ParameterVariance.Contravariant => other is ParameterVariance.Contravariant or ParameterVariance.Independent or ParameterVariance.Invariant,
            ParameterVariance.Invariant => other is ParameterVariance.Invariant or ParameterVariance.Independent,
            ParameterVariance.Independent => other is ParameterVariance.Independent or ParameterVariance.Invariant,
            ParameterVariance.Covariant
                => other is ParameterVariance.Covariant or ParameterVariance.Independent or ParameterVariance.Invariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };

    internal static ParameterVariance Antivariance(this ParameterVariance variance)
        => variance switch
        {
            ParameterVariance.Contravariant => ParameterVariance.Covariant,
            ParameterVariance.Invariant => ParameterVariance.Invariant,
            // TODO not sure if this is correct for Independent
            ParameterVariance.Independent => ParameterVariance.Independent,
            ParameterVariance.Covariant => ParameterVariance.Contravariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
}
