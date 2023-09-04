using System.Numerics;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout
{
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct AzothValue
    {
        [FieldOffset(0)] public readonly AzothObject ObjectValue;
        [FieldOffset(0)] public readonly BigInteger IntValue;
        [FieldOffset(0)] public readonly byte[] BytesValue;
        [FieldOffset(0)] private readonly ValueType value;

        public bool IsNone => value.Struct is null;
        public bool BoolValue => value.Simple.BoolValue;
        public byte ByteValue => value.Simple.ByteValue;
        public int I32Value => value.Simple.I32Value;
        public uint U32Value => value.Simple.U32Value;
        public nint OffsetValue => value.Simple.OffsetValue;
        public nuint SizeValue => value.Simple.SizeValue;

        #region Static Factory Methods/Properties
        public static readonly AzothValue None = default;

        public static AzothValue Object(AzothObject value) => new(value);
        public static AzothValue Int(BigInteger value) => new(value);
        public static AzothValue Bytes(byte[] value) => new(value);
        public static AzothValue Bool(bool value) => new(value);
        public static AzothValue Byte(byte value) => new(value);
        public static AzothValue I32(int value) => new(value);
        public static AzothValue U32(uint value) => new(value);
        public static AzothValue Offset(nint value) => new(value);

        public static AzothValue Size(nuint value) => new(value);

        #endregion

        #region Private Constructors
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private AzothValue(AzothObject value) : this()
        {
            ObjectValue = value;
        }
        private AzothValue(BigInteger value) : this()
        {
            IntValue = value;
        }
        private AzothValue(byte[] value) : this()
        {
            BytesValue = value;
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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
