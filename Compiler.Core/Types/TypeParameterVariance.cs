using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Types;

/// <summary>
/// The variance that can be declared on a type parameter.
/// </summary>
public enum TypeParameterVariance
{
    Contravariant = -1,
    Invariant = 0,
    ReadOnlyCovariant = 1,
    Covariant = 2,
}

public static class TypeParameterVarianceExtensions
{
    public static string ToSourceCodeString(this TypeParameterVariance variance)
        => variance switch
        {
            TypeParameterVariance.Contravariant => "in",
            TypeParameterVariance.Invariant => "",
            TypeParameterVariance.ReadOnlyCovariant => "readonly out",
            TypeParameterVariance.Covariant => "out",
            _ => throw ExhaustiveMatch.Failed(variance),
        };

    /// <param name="readOnlySelf">Whether the self parameter type is readonly.
    /// <see langword="null"/> is used for base types to indicate that it could behave either way.</param>
    public static TypeVariance? ToTypeVariance(this TypeParameterVariance variance, bool? readOnlySelf)
    {
        if (readOnlySelf is bool knownReadOnlySelf)
            return variance.ToTypeVariance(knownReadOnlySelf);
        return variance switch
        {
            TypeParameterVariance.Contravariant => TypeVariance.Contravariant,
            TypeParameterVariance.Invariant => TypeVariance.Invariant,
            TypeParameterVariance.ReadOnlyCovariant => null,
            TypeParameterVariance.Covariant => TypeVariance.Covariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
    }

    /// <param name="readOnlySelf">Whether the self parameter type is readonly.</param>
    public static TypeVariance ToTypeVariance(this TypeParameterVariance variance, bool readOnlySelf)
        => variance switch
        {
            TypeParameterVariance.Contravariant => TypeVariance.Contravariant,
            TypeParameterVariance.Invariant => TypeVariance.Invariant,
            TypeParameterVariance.ReadOnlyCovariant
                // NonwritableCovariant acts like Covariant or Invariant depending on whether self is readonly
                => readOnlySelf ? TypeVariance.Covariant : TypeVariance.Invariant,
            TypeParameterVariance.Covariant => TypeVariance.Covariant,
            _ => throw ExhaustiveMatch.Failed(variance),
        };
}
