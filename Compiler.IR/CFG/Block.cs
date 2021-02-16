using System.Collections.Generic;
using Azoth.Tools.Bootstrap.IL.Instructions;

namespace Azoth.Tools.Bootstrap.Compiler.IR.CFG
{
    public class Block
    {
        private readonly List<Instruction> instructions = new List<Instruction>();
        public IEnumerable<Instruction> Instructions { get; }

        public Block()
        {
            Instructions = instructions.AsReadOnly();
        }

        public ushort Add(Instruction instruction)
        {
            var index = (ushort)instructions.Count;
            instructions.Add(instruction);
            return index;
        }
    }
}
