using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class CompareInstruction : InstructionWithResult
    {
        public CompareInstructionOperator Operator { get; }
        public NumericType Type { get; }
        public Operand LeftOperand { get; }
        public Operand RightOperand { get; }

        public CompareInstruction(
            Place resultPlace,
            CompareInstructionOperator @operator,
            NumericType type,
            Operand leftOperand,
            Operand rightOperand,
            Scope scope)
            : base(resultPlace, TextSpan.Covering(leftOperand.Span, rightOperand.Span), scope)
        {
            Operator = @operator;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Type = type;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = {Operator.ToInstructionString()}[{Type}] {LeftOperand}, {RightOperand}";
        }
    }
}
