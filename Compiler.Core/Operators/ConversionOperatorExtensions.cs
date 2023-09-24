using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Operators;

public static class ConversionOperatorExtensions
{
    public static string ToSymbolString(this ConversionOperator @operator) =>
        @operator switch
        {
            ConversionOperator.Safe => "as",
            ConversionOperator.Aborting => "as!",
            ConversionOperator.Optional => "as?",
            _ => throw ExhaustiveMatch.Failed(@operator)
        };
}
