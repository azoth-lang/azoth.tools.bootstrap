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

    internal static TypeVariance ToTypeVariance(this ParameterVariance variance)
        => variance switch
        {
            ParameterVariance.Contravariant => TypeVariance.Contravariant,
            ParameterVariance.Invariant => TypeVariance.Invariant,
            ParameterVariance.Independent => TypeVariance.Invariant,
            ParameterVariance.Covariant => TypeVariance.Covariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
}
