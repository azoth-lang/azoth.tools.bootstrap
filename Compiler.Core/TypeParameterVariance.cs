using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

/// <summary>
/// The variance that can be declared on a type parameter.
/// </summary>
public enum TypeParameterVariance
{
    Contravariant = -1,
    Invariant = 0,
    NonwritableCovariant = 1,
    Covariant = 2,
}

public static class TypeParameterVarianceExtensions
{
    public static string ToSourceCodeString(this TypeParameterVariance variance)
        => variance switch
        {
            TypeParameterVariance.Contravariant => "in",
            TypeParameterVariance.Invariant => "",
            TypeParameterVariance.NonwritableCovariant => "nonwriteable out",
            TypeParameterVariance.Covariant => "out",
            _ => throw ExhaustiveMatch.Failed(variance),
        };

    /// <param name="nonwritableSelf">Whether the self parameter type is nonwriteable.
    /// <see langword="null"/> is used for base types to indicate that it could behave either way.</param>
    public static TypeVariance? ToTypeVariance(this TypeParameterVariance variance, bool? nonwritableSelf)
    {
        if (nonwritableSelf is bool knownNonwritableSelf)
            return ToTypeVariance(variance, knownNonwritableSelf);
        return variance switch
        {
            TypeParameterVariance.Contravariant => TypeVariance.Contravariant,
            TypeParameterVariance.Invariant => TypeVariance.Invariant,
            TypeParameterVariance.NonwritableCovariant => null,
            TypeParameterVariance.Covariant => TypeVariance.Covariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
    }

    /// <param name="nonwritableSelf">Whether the self parameter type is nonwriteable.</param>
    public static TypeVariance ToTypeVariance(this TypeParameterVariance variance, bool nonwritableSelf)
        => variance switch
        {
            TypeParameterVariance.Contravariant => TypeVariance.Contravariant,
            TypeParameterVariance.Invariant => TypeVariance.Invariant,
            TypeParameterVariance.NonwritableCovariant
                // NonwritableCovariant acts like Covariant or Invariant depending on whether self is nonwritable
                => nonwritableSelf ? TypeVariance.Covariant : TypeVariance.Invariant,
            TypeParameterVariance.Covariant => TypeVariance.Covariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
}
