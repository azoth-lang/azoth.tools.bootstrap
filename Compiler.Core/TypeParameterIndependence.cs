using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

public enum TypeParameterIndependence
{
    None = 0,
    SharableIndependent,
    Independent,
}

public static class TypeParameterIndependenceExtensions
{
    public static string ToSourceCodeString(this TypeParameterIndependence independence)
        => independence switch
        {
            TypeParameterIndependence.None => "",
            TypeParameterIndependence.SharableIndependent => "sharable ind",
            TypeParameterIndependence.Independent => "ind",
            _ => throw ExhaustiveMatch.Failed(independence),
        };
}
