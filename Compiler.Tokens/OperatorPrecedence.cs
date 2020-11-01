namespace Azoth.Tools.Bootstrap.Compiler.Tokens
{
    public enum OperatorPrecedence
    {
        Min = Assignment, // The minimum precedence
        AboveAssignment = Coalesce,
        Assignment = 1, // `=` `+=` `-=`
        Coalesce, // `??`
        LogicalOr, // `or`
        LogicalAnd, // `and`
        Equality, // `==` `≠`
        Relational, // `<` `<=` `>` `>=` `<:`
        Range, // `..` `..<`
        Additive, // `+` `-`
        Multiplicative, // `*` `/`
        Unary, // `+` `-` `not` `?`
        Primary // `f()` `.` `[]`
    }
}
