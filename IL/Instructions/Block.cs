using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.IL.Instructions
{
    public class Block
    {
        public FixedList<BlockParameter> Parameters { get; }
        public FixedList<Instruction> Instructions { get; }

        public Block(
            FixedList<BlockParameter> parameters,
            FixedList<Instruction> instructions)
        {
            Instructions = instructions;
            Parameters = parameters;
        }
    }
}
