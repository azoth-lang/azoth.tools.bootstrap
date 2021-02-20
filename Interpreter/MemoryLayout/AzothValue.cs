using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Interpreter.MemoryLayout
{
    [StructLayout(LayoutKind.Explicit)]
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
