using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Operators;

public static class UnaryOperatorExtensions
{
    public static string ToSymbolString(this UnaryOperator @operator)
    {
        return @operator switch
        {
            UnaryOperator.Not => "not ",
            UnaryOperator.Minus => "-",
            UnaryOperator.Plus => "+",
            _ => throw ExhaustiveMatch.Failed(@operator)
        };
    }
}
