using System;

namespace Azoth.Tools.Bootstrap.Interpreter.MemoryLayout
{
    public readonly struct AzothObject
    {
        private readonly AzothObjectSlot[] slots;

        public AzothObject(int dataLength, int slotCount)
        {
            slots = new AzothObjectSlot[slotCount + 1];
            slots[0].Data = dataLength == 0 ? Array.Empty<byte>() : new byte[dataLength];
        }

        public byte[] Data => slots[0].Data;
        public Span<AzothObjectSlot> Slots => new Span<AzothObjectSlot>(slots, 1, slots.Length - 1);
    }
}
