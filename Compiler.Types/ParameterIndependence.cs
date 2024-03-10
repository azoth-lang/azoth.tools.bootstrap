using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public enum ParameterIndependence
{
    None = 0,
    SharableIndependent,
    Independent,
}

public static class ParameterIndependenceExtensions
{
    public static string ToSourceCodeString(this ParameterIndependence independence)
        => independence switch
        {
            ParameterIndependence.None => "",
            ParameterIndependence.SharableIndependent => "sharable ind",
            ParameterIndependence.Independent => "ind",
            _ => throw ExhaustiveMatch.Failed(independence),
        };
}
