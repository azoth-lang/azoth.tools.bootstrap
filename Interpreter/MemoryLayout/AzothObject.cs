using System;

namespace Azoth.Tools.Bootstrap.Interpreter.MemoryLayout
{
    /// <summary>
    /// An object created by Azoth IL
    /// </summary>
    /// <remarks>Two AzothObjects are equal if they reference the same object</remarks>
    public readonly struct AzothObject : IEquatable<AzothObject>
    {
        private readonly AzothObjectSlot[] slots;

        public AzothObject(int dataLength, int slotCount)
        {
            slots = new AzothObjectSlot[slotCount + 1];
            slots[0].Data = dataLength == 0 ? Array.Empty<byte>() : new byte[dataLength];
        }

        //public byte[] Data => slots[0].Data;
        public Span<AzothObjectSlot> Slots => new Span<AzothObjectSlot>(slots, 1, slots.Length - 1);

        public bool Equals(AzothObject other)
        {
            return ReferenceEquals(slots, other.slots);
        }

        public override bool Equals(object? obj)
        {
            return obj is AzothObject other && Equals(other);
        }

        public override int GetHashCode()
        {
            return slots.GetHashCode();
        }

        public static bool operator ==(AzothObject left, AzothObject right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AzothObject left, AzothObject right)
        {
            return !left.Equals(right);
        }
    }
}
