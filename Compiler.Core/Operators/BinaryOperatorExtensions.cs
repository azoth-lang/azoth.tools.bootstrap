using System;
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
            BinaryOperator.ReferenceEquals => "@==",
            BinaryOperator.NotReferenceEqual => "@=/=",
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
            BinaryOperator.QuestionQuestion => "??",
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

            BinaryOperator.EqualsEquals => OperatorPrecedence.Equality,
            BinaryOperator.NotEqual => OperatorPrecedence.Equality,

            BinaryOperator.ReferenceEquals => OperatorPrecedence.Relational,
            BinaryOperator.NotReferenceEqual => OperatorPrecedence.Relational,
            BinaryOperator.LessThan => OperatorPrecedence.Relational,
            BinaryOperator.LessThanOrEqual => OperatorPrecedence.Relational,
            BinaryOperator.GreaterThan => OperatorPrecedence.Relational,
            BinaryOperator.GreaterThanOrEqual => OperatorPrecedence.Relational,

            BinaryOperator.QuestionQuestion => OperatorPrecedence.Coalesce,

            _ => throw ExhaustiveMatch.Failed(@operator),
        };

    public static bool IsRangeOperator(this BinaryOperator @operator)
        => @operator switch
        {
            BinaryOperator.DotDot
                or BinaryOperator.DotDotLessThan
                or BinaryOperator.LessThanDotDot
                or BinaryOperator.LessThanDotDotLessThan => true,
            _ => false,
        };

    public static bool RangeInclusiveOfStart(this BinaryOperator @operator) =>
        @operator switch
        {
            BinaryOperator.DotDot or BinaryOperator.DotDotLessThan => true,
            BinaryOperator.LessThanDotDot or BinaryOperator.LessThanDotDotLessThan => false,
            _ => throw new InvalidOperationException($"`{@operator.ToSymbolString()}` is not a range operator."),
        };

    public static bool RangeInclusiveOfEnd(this BinaryOperator @operator) =>
        @operator switch
        {
            BinaryOperator.DotDot or BinaryOperator.LessThanDotDot => true,
            BinaryOperator.DotDotLessThan or BinaryOperator.LessThanDotDotLessThan => false,
            _ => throw new InvalidOperationException($"`{@operator.ToSymbolString()}` is not a range operator."),
        };
}
