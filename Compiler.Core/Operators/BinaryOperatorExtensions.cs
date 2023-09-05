using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Operators;

public static class BinaryOperatorExtensions
{
    public static string ToSymbolString(this BinaryOperator @operator)
        => @operator switch
        {
            BinaryOperator.Plus => "+",
            BinaryOperator.Minus => "-",
            BinaryOperator.Asterisk => "*",
            BinaryOperator.Slash => "/",
            BinaryOperator.EqualsEquals => "==",
            BinaryOperator.NotEqual => "=/=",
            BinaryOperator.LessThan => "<",
            BinaryOperator.LessThanOrEqual => "<=",
            BinaryOperator.GreaterThan => ">",
            BinaryOperator.GreaterThanOrEqual => ">=",
            BinaryOperator.And => "and",
            BinaryOperator.Or => "or",
            BinaryOperator.DotDot => "..",
            BinaryOperator.LessThanDotDot => "<..",
            BinaryOperator.DotDotLessThan => "..<",
            BinaryOperator.LessThanDotDotLessThan => "<..<",
            BinaryOperator.As => "as",
            BinaryOperator.AsExclamation => "as!",
            BinaryOperator.AsQuestion => "as?",
            _ => throw ExhaustiveMatch.Failed(@operator)
        };

    public static OperatorPrecedence Precedence(this BinaryOperator @operator)
        => @operator switch
        {
            BinaryOperator.And => OperatorPrecedence.LogicalAnd,
            BinaryOperator.Or => OperatorPrecedence.LogicalOr,

            BinaryOperator.Plus => OperatorPrecedence.Additive,
            BinaryOperator.Minus => OperatorPrecedence.Additive,

            BinaryOperator.Asterisk => OperatorPrecedence.Multiplicative,
            BinaryOperator.Slash => OperatorPrecedence.Multiplicative,

            BinaryOperator.DotDot => OperatorPrecedence.Range,
            BinaryOperator.DotDotLessThan => OperatorPrecedence.Range,
            BinaryOperator.LessThanDotDot => OperatorPrecedence.Range,
            BinaryOperator.LessThanDotDotLessThan => OperatorPrecedence.Range,

            BinaryOperator.EqualsEquals => OperatorPrecedence.Relational,
            BinaryOperator.NotEqual => OperatorPrecedence.Relational,
            BinaryOperator.LessThan => OperatorPrecedence.Relational,
            BinaryOperator.LessThanOrEqual => OperatorPrecedence.Relational,
            BinaryOperator.GreaterThan => OperatorPrecedence.Relational,
            BinaryOperator.GreaterThanOrEqual => OperatorPrecedence.Relational,

            BinaryOperator.As => OperatorPrecedence.Conversion,
            BinaryOperator.AsExclamation => OperatorPrecedence.Conversion,
            BinaryOperator.AsQuestion => OperatorPrecedence.Conversion,

            _ => throw ExhaustiveMatch.Failed(@operator),
        };
}
