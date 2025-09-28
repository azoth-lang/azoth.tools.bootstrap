using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Types;

public enum TypeParameterIndependence
{
    None = 0,
    ShareableIndependent,
    Independent,
}

public static class TypeParameterIndependenceExtensions
{
    public static string ToSourceCodeString(this TypeParameterIndependence independence)
        => independence switch
        {
            TypeParameterIndependence.None => "",
            TypeParameterIndependence.ShareableIndependent => "independent(shareable)",
            TypeParameterIndependence.Independent => "independent",
            _ => throw ExhaustiveMatch.Failed(independence),
        };
}
