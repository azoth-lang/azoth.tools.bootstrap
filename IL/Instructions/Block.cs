using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.IL.Instructions
{
    public class Block
    {
        public FixedList<Instruction> Instructions { get; }

        public Block(FixedList<Instruction> instructions)
        {
            Instructions = instructions;
        }
    }
}
