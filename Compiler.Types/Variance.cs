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
}
