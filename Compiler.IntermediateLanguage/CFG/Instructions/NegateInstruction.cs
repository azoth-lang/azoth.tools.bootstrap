using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class NegateInstruction : InstructionWithResult
    {
        public Operand Operand { get; }
        public NumericType Type { get; }

        public NegateInstruction(
            Place resultPlace,
            NumericType type,
            Operand operand,
            TextSpan span,
            Scope scope)
            : base(resultPlace, span, scope)
        {
            Type = type;
            Operand = operand;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = NEG[{Type}] {Operand}";
        }
    }
}
