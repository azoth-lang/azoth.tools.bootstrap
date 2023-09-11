using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;

[StructLayout(LayoutKind.Explicit)]
internal readonly struct AzothValue
{
    [FieldOffset(0)] public readonly AzothObject ObjectValue;
    [FieldOffset(0)] public readonly BigInteger IntValue;
    [FieldOffset(0)] public readonly byte[] BytesValue;
    [FieldOffset(0)] public readonly RawBoundedList RawBoundedListValue;
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
    public static AzothValue RawBoundedList(nuint capacity) => new(new RawBoundedList(capacity));
    public static AzothValue RawBoundedList(nuint count, AzothValue[] items) => new(new RawBoundedList(count, items));
    public static AzothValue Bool(bool value) => new(value);
    public static AzothValue Byte(byte value) => new(value);
    public static AzothValue I32(int value) => new(value);
    public static AzothValue U32(uint value) => new(value);
    public static AzothValue Offset(nint value) => new(value);
    public static AzothValue Size(nuint value) => new(value);
    #endregion

    #region Private Constructors
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private AzothValue(AzothObject value)
    {
        ObjectValue = value;
    }
    private AzothValue(BigInteger value)
    {
        IntValue = value;
    }
    private AzothValue(byte[] value)
    {
        BytesValue = value;
    }
    private AzothValue(RawBoundedList value)
    {
        RawBoundedListValue = value;
    }
    private AzothValue(bool value)
    {
        this.value.Struct = NotStruct;
        this.value.Simple.BoolValue = value;
    }

    private AzothValue(byte value)
    {
        this.value.Struct = NotStruct;
        this.value.Simple.ByteValue = value;
    }
    private AzothValue(int value)
    {
        this.value.Struct = NotStruct;
        this.value.Simple.I32Value = value;
    }
    private AzothValue(uint value)
    {
        this.value.Struct = NotStruct;
        this.value.Simple.U32Value = value;
    }
    private AzothValue(nint value)
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

    public AzothValue Increment(NumericType numberType)
    {
        if (numberType == DataType.Int || numberType == DataType.UInt) return Int(IntValue + 1);
        if (numberType == DataType.Byte) return Byte((byte)(ByteValue + 1));
        if (numberType == DataType.Size) return Size(SizeValue + 1);
        if (numberType == DataType.Offset) return Offset(OffsetValue + 1);
        if (numberType == DataType.Int32) return I32(I32Value + 1);
        if (numberType == DataType.UInt32) return U32(U32Value + 1);

        throw new NotImplementedException($"Increment of {numberType}");
    }

    public AzothValue Decrement(NumericType numberType)
    {
        if (numberType == DataType.Int || numberType == DataType.UInt) return Int(IntValue - 1);
        if (numberType == DataType.Byte) return Byte((byte)(ByteValue - 1));
        if (numberType == DataType.Size) return Size(SizeValue - 1);
        if (numberType == DataType.Offset) return Offset(OffsetValue - 1);
        if (numberType == DataType.Int32) return I32(I32Value - 1);
        if (numberType == DataType.UInt32) return U32(U32Value - 1);

        throw new NotImplementedException($"Decrement of {numberType}");
    }

    public BigInteger ToBigInteger(NumericType numberType)
    {
        if (numberType == DataType.Int || numberType == DataType.UInt) return IntValue;
        if (numberType == DataType.Byte) return ByteValue;
        if (numberType == DataType.Size) return SizeValue;
        if (numberType == DataType.Offset) return OffsetValue;
        if (numberType == DataType.Int32) return I32Value;
        if (numberType == DataType.UInt32) return U32Value;

        throw new NotImplementedException($"ToBigInteger for {numberType}");
    }

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

    private static readonly object NotStruct = new();
}
