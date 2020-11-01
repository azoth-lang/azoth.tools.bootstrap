using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.TerminatorInstructions;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Block
    {
        public int Number { get; }
        public FixedList<Instruction> Instructions { get; }
        public TerminatorInstruction Terminator { get; }

        public Block(int number, FixedList<Instruction> instructions, TerminatorInstruction terminator)
        {
            Number = number;
            Instructions = instructions;
            Terminator = terminator;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"BB #{Number}, Instructions={Instructions.Count+1}";
    }
}
