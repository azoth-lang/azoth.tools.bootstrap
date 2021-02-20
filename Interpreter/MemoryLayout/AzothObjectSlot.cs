using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Interpreter.MemoryLayout
{
    [StructLayout(LayoutKind.Explicit)]
    public struct AzothObjectSlot
    {
        [FieldOffset(0)] public AzothReference Reference;
        [SuppressMessage("Naming", "CA1720:Identifier contains type name",
            Justification = "This is an Azoth int")]
        [FieldOffset(0)] public BigInteger Int;
        [FieldOffset(0)] public byte[] Data;
    }
}
