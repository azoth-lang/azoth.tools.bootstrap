using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

public enum TypeVariance
{
    Contravariant = -1,
    Invariant = 0,
    Covariant = 1,
}

public static class TypeVarianceExtensions
{
    /// <summary>
    /// Is the given variance compatible with this one?
    /// </summary>
    /// <param name="nonwritableSelf">Whether the self parameter type is nonwriteable.
    /// <see langword="null"/> is used for base types to indicate that it could behave either way.</param>
    public static bool CompatibleWith(this TypeVariance contextVariance, ParameterVariance parameterVariance, bool? nonwritableSelf)
    {
        // if NonwritableCovariant could behave either way, then the most restrictive is used
        var variance = parameterVariance.ToVariance(nonwritableSelf) ?? TypeVariance.Covariant;
        return contextVariance switch
        {
            TypeVariance.Contravariant => variance <= TypeVariance.Invariant,
            TypeVariance.Invariant => variance == TypeVariance.Invariant,
            TypeVariance.Covariant => variance >= TypeVariance.Invariant,
            _ => throw ExhaustiveMatch.Failed(contextVariance),
        };
    }

    public static TypeVariance Inverse(this TypeVariance variance)
        => (TypeVariance)(-(int)variance);

    public static string ToSourceCodeString(this TypeVariance variance)
        => variance switch
        {
            TypeVariance.Contravariant => "in",
            TypeVariance.Invariant => "",
            TypeVariance.Covariant => "out",
            _ => throw ExhaustiveMatch.Failed(variance),
        };

}
