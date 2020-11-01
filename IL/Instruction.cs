namespace Azoth.Tools.Bootstrap.IL
{
    public readonly struct Instruction
    {
        private readonly int value;

        public Instruction(OpCode opCode, int operand1, int operand2)
        {
            value = (int)opCode;
        }
    }
}
