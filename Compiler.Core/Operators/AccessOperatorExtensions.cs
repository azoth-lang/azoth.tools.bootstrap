using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Operators;

public static class AccessOperatorExtensions
{
    public static string ToSymbolString(this AccessOperator @operator)
    {
        return @operator switch
        {
            AccessOperator.Standard => ".",
            AccessOperator.Conditional => "?.",
            _ => throw ExhaustiveMatch.Failed(@operator)
        };
    }
}
