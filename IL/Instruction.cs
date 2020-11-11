using System;

namespace Azoth.Tools.Bootstrap.IL
{
    public readonly struct Instruction : IEquatable<Instruction>
    {
        private readonly int value;

        public Instruction(OpCode opCode, int operand1, int operand2)
        {
            // TODO correctly build instruction
            value = (int)opCode + operand1 + operand2;
        }

        public bool Equals(Instruction other)
        {
            return value == other.value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Instruction other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public static bool operator ==(Instruction left, Instruction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Instruction left, Instruction right)
        {
            return !left.Equals(right);
        }
    }
}
