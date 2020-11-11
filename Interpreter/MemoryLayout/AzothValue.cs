using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Interpreter.MemoryLayout
{
    [StructLayout(LayoutKind.Explicit)]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types",
        Justification = "Not possible to compare equality because union type is unknown")]
    public struct AzothValue
    {
        [FieldOffset(0)] public int I32;
        [FieldOffset(0)] public uint U32;
        [FieldOffset(0)] public long I64;
        [FieldOffset(0)] public ulong U64;
        [FieldOffset(0)] public float F32;
        [FieldOffset(0)] public double F64;
    }
}
