using System;
using System.ComponentModel;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.IL.Instructions
{
    public readonly struct Instruction : IEquatable<Instruction>
    {
        private const int Segment1Offset = 24;
        private const uint Segment2Mask = 0x00FFF000;
        private const int Segment2Offset = 12;
        private const uint Segment3Mask = 0x00000FFF;
        private const uint MaxOperandValue = 0x00000FFF;
        private const uint ValueMask = 0x00FFFFFF;
        private const uint MaxUnsignedValue = 0x00FFFFFF;

        private readonly uint value;

        public Instruction(ShortOpcode shortOpcode, ushort operand1, ushort operand2)
        {
            if (!shortOpcode.IsDefined())
                throw new InvalidEnumArgumentException(nameof(shortOpcode), (int)shortOpcode, typeof(ShortOpcode));
            if (operand1 > MaxOperandValue)
                throw new ArgumentException("Invalid operand value", nameof(operand1));
            if (operand2 > MaxOperandValue)
                throw new ArgumentException("Invalid operand value", nameof(operand2));
            value = (uint)shortOpcode << Segment1Offset | (uint)operand1 << Segment2Offset | operand2;
        }

        public Instruction(ShortOpcode shortOpcode, uint value)
        {
            if (!shortOpcode.IsDefined())
                throw new InvalidEnumArgumentException(nameof(shortOpcode), (int)shortOpcode, typeof(ShortOpcode));
            if (value > MaxUnsignedValue)
                throw new ArgumentException("Invalid operand value", nameof(value));
            this.value = (uint)shortOpcode << Segment1Offset | value;
        }

        public Instruction(NullaryOpcode opcode)
        {
            if (!opcode.IsDefined())
                throw new InvalidEnumArgumentException(nameof(opcode), (int)opcode, typeof(NullaryOpcode));
            value = (uint)opcode;
        }

        public ShortOpcode ShortOpcode => (ShortOpcode)(value >> Segment1Offset);
        public ushort Operand1 => (ushort)((value & Segment2Mask) >> Segment2Offset);
        public ushort Operand2 => (ushort)(value & Segment3Mask);
        public uint UnsignedValue => value & ValueMask;
        /// <remarks>Shift up unsigned, then shift down signed to get sign extension</remarks>
        public int SignedValue => (int)((value & ValueMask) << 8) >> 8;

        public UnaryOpcode UnaryOpcode => (UnaryOpcode)((value & Segment2Mask) >> Segment2Offset);
        public ushort UnaryOperand => (ushort)(value & Segment3Mask);

        public NullaryOpcode NullaryOpcode => (NullaryOpcode)(value & Segment3Mask);

        public bool Equals(Instruction other)
        {
            return value == other.value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Instruction other && value == other.value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public static bool operator ==(Instruction left, Instruction right)
        {
            return left.value == right.value;
        }

        public static bool operator !=(Instruction left, Instruction right)
        {
            return left.value != right.value;
        }
    }
}
