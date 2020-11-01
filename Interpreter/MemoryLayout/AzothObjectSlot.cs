using System.Numerics;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Interpreter.MemoryLayout
{
    [StructLayout(LayoutKind.Explicit)]
    public struct AzothObjectSlot
    {
        [FieldOffset(0)] public AzothReference Reference;
        [FieldOffset(0)] public BigInteger Int;
        [FieldOffset(0)] public byte[] Data;
    }
}
