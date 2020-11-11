using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Interpreter.MemoryLayout
{
    [StructLayout(LayoutKind.Explicit)]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types",
        Justification = "Not possible to compare equality because union type is unknown")]
    public struct AzothObjectSlot
    {
        [FieldOffset(0)] public AzothReference Reference;
        [SuppressMessage("Naming", "CA1720:Identifier contains type name",
            Justification = "This is an Azoth int")]
        [FieldOffset(0)] public BigInteger Int;
        [FieldOffset(0)] public byte[] Data;
    }
}
