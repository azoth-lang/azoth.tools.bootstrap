namespace Azoth.Tools.Bootstrap.Compiler.Core.Operators;

public enum OperatorPrecedence
{
    Min = Assignment, // The minimum precedence
    AboveAssignment = Coalesce,
    Assignment = 1, // `=` `+=` `-=` `*=` `/=`
    Coalesce, // `??`
    LogicalOr, // `or`
    LogicalAnd, // `and`
    Equality, // `==` `≠`
    Relational, // `<` `<=` `>` `>=` `<:` `is`
    Range, // `..` `..<` `<..` `<..<`
    Conversion, // `as` `as!` `as?`
    Additive, // `+` `-`
    Multiplicative, // `*` `/`
    Unary, // `+` `-` `not` `?`
    Primary // `f()` `.` `[]`
}
