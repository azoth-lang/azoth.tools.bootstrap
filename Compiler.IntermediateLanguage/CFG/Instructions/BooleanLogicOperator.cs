using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public enum BooleanLogicOperator
    {
        And = 1,
        Or,
    }

    public static class BooleanLogicOperatorExtensions
    {
        public static string ToInstructionString(this BooleanLogicOperator @operator)
        {
            return @operator switch
            {
                BooleanLogicOperator.And => "AND",
                BooleanLogicOperator.Or => "OR",
                _ => throw ExhaustiveMatch.Failed(@operator)
            };
        }
    }
}
