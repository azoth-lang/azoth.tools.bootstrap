using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Operators;

public static class AssignmentOperatorExtensions
{
    public static string ToSymbolString(this AssignmentOperator @operator)
    {
        return @operator switch
        {
            AssignmentOperator.Simple => "=",
            AssignmentOperator.Plus => "+=",
            AssignmentOperator.Minus => "-=",
            AssignmentOperator.Asterisk => "*=",
            AssignmentOperator.Slash => "/=",
            _ => throw ExhaustiveMatch.Failed(@operator)
        };
    }
}
