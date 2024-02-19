using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public enum Variance
{
    Contravariant = -1,
    Invariant = 0,
    Independent = 1,
    Covariant = 2,
}

public static class VarianceExtensions
{
    public static string ToSourceCodeString(this Variance variance) =>
        variance switch
        {
            Variance.Contravariant => "in",
            Variance.Invariant => "",
            Variance.Independent => "ind",
            Variance.Covariant => "out",
            _ => throw ExhaustiveMatch.Failed(variance),
        };

    internal static bool CompatibleWith(this Variance variance, Variance other)
        => variance switch
        {
            Variance.Contravariant => other is Variance.Contravariant or Variance.Invariant,
            Variance.Invariant => other is Variance.Invariant,
            Variance.Independent => other is Variance.Independent or Variance.Invariant,
            Variance.Covariant
                => other is Variance.Covariant or Variance.Independent or Variance.Invariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };

    internal static Variance Antivariance(this Variance variance)
        => variance switch
        {
            Variance.Contravariant => Variance.Covariant,
            Variance.Invariant => Variance.Invariant,
            // TODO not sure if this is correct for Independent
            Variance.Independent => Variance.Independent,
            Variance.Covariant => Variance.Contravariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
}
