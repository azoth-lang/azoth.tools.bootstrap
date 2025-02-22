namespace Azoth.Tools.Bootstrap.Compiler.Core.Operators;

public enum OperatorPrecedence
{
    Min = Assignment, // The minimum precedence
    AboveAssignment = Coalesce,
    Assignment = 1, // `=` `+=` `-=` `*=` `/=`
    Coalesce, // `??`
    LogicalOr, // `or`
    LogicalAnd, // `and`
    LogicalNot, // `not`
    Equality, // `==` `=/=`
    // Note `@==` `=/=` are relational, not equality because they act more like immediate
    // comparisons that you would then want to further compare the results of.
    Relational, // `<` `<=` `>` `>=` `@==` `=/=` `<:` `is`
    Range, // `..` `..<` `<..` `<..<`
    Conversion, // `as` `as!` `as?`
    Additive, // `+` `-`
    Multiplicative, // `*` `/`
    Unary, // `+` `-` `await` `move` `freeze`
    Primary // `f()` `.` `[]`
}
