namespace Azoth.Tools.Bootstrap.Compiler.CST;

// Record Types:
// 1. Value Equality
// 2. `with` Expressions
// 3. ~Immutability
// 4. Positional Syntax

public static class Algebraic
{
    public record BoolLiteral(bool IsCool, string Syntax, bool Value);

    public record BinaryOperatorExpression<TExpression>(
        bool IsCool,
        string Syntax,
        TExpression Left,
        string Operator,
        TExpression Right);


}

public class L0<TExpression>
{
    public record BoolLiteral(bool IsCool, string Syntax, bool Value) : L0.Expression(IsCool)
    {
        public override string Syntax { get; } = Syntax;
    }

    public record BinaryOperatorExpression(
        bool IsCool,
        string Syntax,
        TExpression Left,
        string Operator,
        TExpression Right) : L0.Expression(IsCool)
    {
        public override string Syntax { get; } = Syntax;
    }

    //public record UnaryOperatorExpression(
    //    bool IsCool,
    //    string Syntax,
    //    string Operator,
    //    TExpression Operand) : TExpression
    //{
    //    public override string Syntax { get; } = Syntax;
    //}
}

public class L0 : L0<L0.Expression>
{
    public abstract record Expression(bool IsCool)
    {
        public abstract object Syntax { get; }
    }
}


public class L1<TExpression> : L0<TExpression>
{
    public record IfExpression(
        bool IsCool,
        string Syntax,
        TExpression Condition,
        TExpression ThenBranch,
        TExpression ElseBranch) : L0.Expression(IsCool)
    {
        public override string Syntax { get; } = Syntax;
    }
}

public class L1 : L1<L1.Expression>
{
    public abstract record Expression(bool IsCool, bool IsBad) : L0.Expression(IsCool);
}


public static class Testing
{
    public static void Test()
    {
        L0.BoolLiteral literal = new(true, "true", true);
        L0.BinaryOperatorExpression binary = new(true, "1 + 2", literal, "+", literal);
        L0.Expression expression = binary with { Left = literal };

        L1.BoolLiteral literal1 = new(true, "true", true);
        //L1.BoolLiteral literal2 = literal;
    }
}


public static class Lang0
{
    // ReSharper disable once InconsistentNaming
    public interface Expression
    {
        object Syntax { get; }
        bool IsCool { get; }
        string ToString();
    }

    public interface IBoolLiteral : Expression
    {
        bool Value { get; }

        public static IBoolLiteral Create(object syntax, bool isCool, bool value)
            => new BoolLiteral(syntax, isCool, value);
    }

    internal record BoolLiteral(object Syntax, bool IsCool, bool Value) : IBoolLiteral
    {
    }

    public interface IBinaryOperatorExpression : Expression
    {
        new string Syntax { get; }
        Expression Left { get; }
        string Operator { get; }
        Expression Right { get; }
    }

    internal record BinaryOperatorExpression(
        string Syntax,
        bool IsCool,
        Expression Left,
        string Operator,
        Expression Right) : IBinaryOperatorExpression
    {
        object Expression.Syntax => Syntax;
    }
}

public static class TestingNew
{
    public static void Test()
    {
        Lang0.IBoolLiteral literal = Lang0.IBoolLiteral.Create("true", true, true);
    }
}

//public record PaymentType
//{

//}

//public record CreditCard(string Name) : PaymentType();

//public record ACH(string AccountNumber, string RoutingNumber) : PaymentType();

//public record Paypal(string Token) : PaymentType();

//public static class Testing
//{
//    public static void Something()
//    {
//        PaymentType payment = new CreditCard("John Doe");
//    }
//}

//public abstract record Foo(DataType Type);

//public record Bar(NonEmptyType Baz) : Foo(Baz);

//final case class Fix[F [_]](unFix: F[Fix[F]])

//public interface IOpen<T> { }

//public record Fix<T, TRecur>(T<Fix<T>> UnFix)
//    where T: IOpen<TRecur>
