using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.IR.CFG
{
    public class ControlFlowGraph
    {
        private readonly List<Block> blocks = new List<Block>();

        public IEnumerable<Block> Blocks { get; }

        public ControlFlowGraph()
        {
            Blocks = blocks.AsReadOnly();
        }

        public Block AddBlock()
        {
            var block = new Block();
            blocks.Add(block);
            return block;
        }
    }
}
