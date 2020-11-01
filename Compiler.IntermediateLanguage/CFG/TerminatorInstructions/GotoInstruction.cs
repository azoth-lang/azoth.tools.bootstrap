using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.TerminatorInstructions
{
    public class GotoInstruction : TerminatorInstruction
    {
        public int BlockNumber { get; }

        public GotoInstruction(int blockNumber, TextSpan span, Scope scope)
            : base(span, scope)
        {
            BlockNumber = blockNumber;
        }

        public override string ToInstructionString()
        {
            return $"GOTO BB #{BlockNumber}";
        }
    }
}
