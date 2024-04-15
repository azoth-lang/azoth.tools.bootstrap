using System;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

public static class Lang1
{
    public interface Expression
    {

    }

    public class BinaryOperatorExpression : Expression
    {
        [SetsRequiredMembers]
        public BinaryOperatorExpression(Expression left, BinaryOperator @operator, Expression right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }
        public Expression Left { get; init; }
        public BinaryOperator Operator { get; }
        public Expression Right { get; init; }
    }
}

public static class Lang2
{
    public interface Expression
    {
        Type Type { get; init; }
    }

    public class BinaryOperatorExpression : Expression
    {
        [SetsRequiredMembers]
        public BinaryOperatorExpression(Expression left, BinaryOperator @operator, Expression right, Type type)
        {
            Left = left;
            Operator = @operator;
            Right = right;
            Type = type;
        }

        public BinaryOperatorExpression(Lang1.BinaryOperatorExpression old)
        {
            Operator = old.Operator;
        }

        public required Expression Left { get; init; }
        public BinaryOperator Operator { get; }
        public required Expression Right { get; init; }
        public required Type Type { get; init; }
    }
}

public static class TestPass
{
    public static Lang2.BinaryOperatorExpression Transform(Lang1.BinaryOperatorExpression from)
    {
        var left = Transform(from.Left);
        return new(from)
        {
            Left = left,
            Right = Transform(from.Right),
            Type = left.Type
        };
    }

    private static Lang2.Expression Transform(Lang1.Expression fromLeft)
    {
        throw new System.NotImplementedException();
    }
}
