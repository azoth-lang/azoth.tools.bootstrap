using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public enum TypeVariance
{
    Contravariant = -1,
    Invariant = 0,
    Covariant = 1,
}

public static class TypeVarianceExtensions
{
    public static string ToSourceCodeString(this TypeVariance variance)
        => variance switch
        {
            TypeVariance.Contravariant => "in",
            TypeVariance.Invariant => "",
            TypeVariance.Covariant => "out",
            _ => throw ExhaustiveMatch.Failed(variance),
        };

    /// <summary>
    /// Is the given variance compatible with this one?
    /// </summary>
    internal static bool CompatibleWith(this TypeVariance variance, TypeVariance other) =>
        variance switch
        {
            TypeVariance.Contravariant => other is TypeVariance.Contravariant or TypeVariance.Invariant,
            TypeVariance.Invariant => other is TypeVariance.Invariant,
            TypeVariance.Covariant => other is TypeVariance.Covariant or TypeVariance.Invariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };

    internal static TypeVariance Inverse(this TypeVariance variance)
        => variance switch
        {
            TypeVariance.Contravariant => TypeVariance.Covariant,
            TypeVariance.Invariant => TypeVariance.Invariant,
            TypeVariance.Covariant => TypeVariance.Contravariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
}
