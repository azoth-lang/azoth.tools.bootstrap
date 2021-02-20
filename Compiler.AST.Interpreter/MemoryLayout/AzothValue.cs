using System.Numerics;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout
{
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct AzothValue
    {
        public static readonly AzothValue None;

        [FieldOffset(0)] public readonly Reference Reference;
        [FieldOffset(0)] public readonly BigInteger Int;
        [FieldOffset(0)] private readonly ValueType value;

        public bool IsNone => value.Struct is null;
        public byte Byte => value.Simple.Byte;
        public int I32 => value.Simple.I32;
        public uint U32 => value.Simple.U32;
        public long I64 => value.Simple.I64;
        public ulong U64 => value.Simple.U64;
        public float F32 => value.Simple.F32;
        public double F64 => value.Simple.F64;

        public AzothValue(BigInteger value) : this()
        {
            Int = value;
        }

        public AzothValue(int value) : this()
        {
            this.value.Simple.I32 = value;
        }

        public AzothValue(object value) : this()
        {
            // TODO handle this better
            this.value.Struct = value;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct SimpleValue
        {
            [FieldOffset(0)] public byte Byte;
            [FieldOffset(0)] public int I32;
            [FieldOffset(0)] public uint U32;
            [FieldOffset(0)] public long I64;
            [FieldOffset(0)] public ulong U64;
            [FieldOffset(0)] public float F32;
            [FieldOffset(0)] public double F64;
        }

        private struct ValueType
        {
            public object? Struct;
            public SimpleValue Simple;
        }

        private static readonly object NotNone = new object();
    }
}
