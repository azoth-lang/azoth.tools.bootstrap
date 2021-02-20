using System.Numerics;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout
{
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct AzothValue
    {
        [FieldOffset(0)] public readonly Reference ReferenceValue;
        [FieldOffset(0)] public readonly BigInteger IntValue;
        [FieldOffset(0)] private readonly ValueType value;

        public bool IsNone => value.Struct is null;
        public bool BoolValue => value.Simple.BoolValue;
        public byte ByteValue => value.Simple.ByteValue;
        public int I32Value => value.Simple.I32Value;
        public uint U32Value => value.Simple.U32Value;
        public nint OffsetValue => value.Simple.OffsetValue;
        public nuint SizeValue => value.Simple.SizeValue;

        #region Static Factory Methods/Properties
        public static readonly AzothValue None;

        public static AzothValue Int(BigInteger value)
        {
            return new AzothValue(value);
        }
        public static AzothValue Bool(bool value)
        {
            return new AzothValue(value);
        }
        public static AzothValue Byte(byte value)
        {
            return new AzothValue(value);
        }
        public static AzothValue I32(int value)
        {
            return new AzothValue(value);
        }
        public static AzothValue U32(uint value)
        {
            return new AzothValue(value);
        }
        public static AzothValue Offset(nint value)
        {
            return new AzothValue(value);
        }
        public static AzothValue Size(nuint value)
        {
            return new AzothValue(value);
        }
        #endregion

        #region Private Constructors
        private AzothValue(BigInteger value) : this()
        {
            IntValue = value;
        }
        private AzothValue(bool value) : this()
        {
            this.value.Struct = NotStruct;
            this.value.Simple.BoolValue = value;
        }

        private AzothValue(byte value) : this()
        {
            this.value.Struct = NotStruct;
            this.value.Simple.ByteValue = value;
        }
        private AzothValue(int value) : this()
        {
            this.value.Struct = NotStruct;
            this.value.Simple.I32Value = value;
        }
        private AzothValue(uint value) : this()
        {
            this.value.Struct = NotStruct;
            this.value.Simple.U32Value = value;
        }
        private AzothValue(nint value) : this()
        {
            this.value.Struct = NotStruct;
            this.value.Simple.OffsetValue = value;
        }
        private AzothValue(nuint value) : this()
        {
            this.value.Struct = NotStruct;
            this.value.Simple.SizeValue = value;
        }
        #endregion

        [StructLayout(LayoutKind.Explicit)]
        private struct SimpleValue
        {
            [FieldOffset(0)] public bool BoolValue;
            [FieldOffset(0)] public byte ByteValue;
            [FieldOffset(0)] public int I32Value;
            [FieldOffset(0)] public uint U32Value;
            //[FieldOffset(0)] public long I64Value;
            //[FieldOffset(0)] public ulong U64Value;
            //[FieldOffset(0)] public float F32Value;
            //[FieldOffset(0)] public double F64Value;
            [FieldOffset(0)] public nint OffsetValue;
            [FieldOffset(0)] public nuint SizeValue;
        }

        private struct ValueType
        {
            public object? Struct;
            public SimpleValue Simple;
        }

        private static readonly object NotStruct = new object();
    }
}
