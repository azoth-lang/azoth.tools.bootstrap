using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.TerminatorInstructions
{
    public class ReturnValueInstruction : TerminatorInstruction
    {
        public Operand Value { get; }

        public ReturnValueInstruction(Operand value, TextSpan span, Scope scope)
            : base(span, scope)
        {
            Value = value;
        }

        public override string ToInstructionString()
        {
            return $"RETURN {Value}";
        }
    }
}
