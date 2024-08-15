using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

public enum ParameterVariance
{
    Contravariant = -1,
    Invariant = 0,
    NonwritableCovariant = 1,
    Covariant = 2,
}

public static class ParameterVarianceExtensions
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

    /// <param name="nonwritableSelf">Whether the self parameter type is nonwriteable.
    /// <see langword="null"/> is used for base types to indicate that it could behave either way.</param>
    public static Variance? ToVariance(this ParameterVariance variance, bool? nonwritableSelf)
    {
        if (nonwritableSelf is bool knownNonwritableSelf)
            return ToVariance(variance, knownNonwritableSelf);
        return variance switch
        {
            ParameterVariance.Contravariant => Variance.Contravariant,
            ParameterVariance.Invariant => Variance.Invariant,
            ParameterVariance.NonwritableCovariant => null,
            ParameterVariance.Covariant => Variance.Covariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
    }

    /// <param name="nonwritableSelf">Whether the self parameter type is nonwriteable.</param>
    public static Variance ToVariance(this ParameterVariance variance, bool nonwritableSelf)
        => variance switch
        {
            ParameterVariance.Contravariant => Variance.Contravariant,
            ParameterVariance.Invariant => Variance.Invariant,
            ParameterVariance.NonwritableCovariant
                // NonwritableCovariant acts like Covariant or Invariant depending on whether self is nonwritable
                => nonwritableSelf ? Variance.Covariant : Variance.Invariant,
            ParameterVariance.Covariant => Variance.Covariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
}
