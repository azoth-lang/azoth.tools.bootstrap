using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;
public static class VarianceExtensions
{
    public static string ToSourceCodeString(this Variance variance)
        => variance switch
        {
            Variance.Contravariant => "in",
            Variance.Invariant => "",
            Variance.Covariant => "out",
            _ => throw ExhaustiveMatch.Failed(variance),
        };
}
