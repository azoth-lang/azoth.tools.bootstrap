using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public enum TypeVariance
{
    Contravariant = -1,
    Invariant = 0,
    NonwritableCovariant = 1,
    Covariant = 2,
}

public static class TypeVarianceExtensions
{
    public static string ToSourceCodeString(this TypeVariance variance)
        => variance switch
        {
            TypeVariance.Contravariant => "in",
            TypeVariance.Invariant => "",
            TypeVariance.NonwritableCovariant => "nonwriteable out",
            TypeVariance.Covariant => "out",
            _ => throw ExhaustiveMatch.Failed(variance),
        };

    /// <summary>
    /// Is the given variance compatible with this one?
    /// </summary>
    internal static bool CompatibleWith(this TypeVariance contextVariance, TypeVariance variance, bool nonwritableSelf)
    {
        // NonwritableCovariant acts like Covariant or Invariant depending on whether self is nonwritable
        if (variance == TypeVariance.NonwritableCovariant)
            variance = nonwritableSelf ? TypeVariance.Covariant : TypeVariance.Invariant;
        return contextVariance switch
        {
            TypeVariance.Contravariant => variance is TypeVariance.Contravariant or TypeVariance.Invariant,
            TypeVariance.Invariant => variance is TypeVariance.Invariant,
            TypeVariance.NonwritableCovariant => variance is TypeVariance.Invariant,
            TypeVariance.Covariant => variance is TypeVariance.Covariant or TypeVariance.Invariant,
            _ => throw ExhaustiveMatch.Failed(contextVariance),
        };
    }

    internal static TypeVariance Inverse(this TypeVariance variance)
        => variance switch
        {
            TypeVariance.Contravariant => TypeVariance.Covariant,
            TypeVariance.Invariant => TypeVariance.Invariant,
            TypeVariance.NonwritableCovariant => TypeVariance.Invariant,
            TypeVariance.Covariant => TypeVariance.Contravariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
}
