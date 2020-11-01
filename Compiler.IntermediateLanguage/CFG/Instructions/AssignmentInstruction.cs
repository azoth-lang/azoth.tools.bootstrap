using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class AssignmentInstruction : InstructionWithResult
    {
        public Operand Operand { get; }

        public AssignmentInstruction(Place resultPlace, Operand operand, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Operand = operand;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = {Operand};";
        }
    }
}
