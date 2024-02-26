using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout.BoundedLists;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ValueType = Azoth.Tools.Bootstrap.Compiler.Types.ValueType;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;

/// <summary>
/// A compact structure for representing Azoth values.
/// </summary>
/// <remarks><para>This struct is laid out in memory so that the first part is a reference and the
/// second is a primitive value. This corresponds with the internal layout of <see cref="BigInteger"/>
/// which stores the bit array in the first part and the <see cref="Int32"/> value second.</para>
///
/// <para>Since small value <see cref="BigInteger"/>s have a null reference, a special flag reference
/// must be used to indicate `none`.</para></remarks>
[StructLayout(LayoutKind.Explicit)]
internal readonly struct AzothValue
{
    private static readonly object NoneFlag = new();

    [FieldOffset(0)] public readonly AzothObject ObjectValue;
    [FieldOffset(0)] public readonly BigInteger IntValue;
    [FieldOffset(0)] public readonly IRawBoundedList RawBoundedListValue;
    [FieldOffset(0)] public readonly Task<AzothValue> PromiseValue;
    [FieldOffset(0)] public readonly FunctionReference FunctionReferenceValue;
    [FieldOffset(0)] private readonly SimpleValueType value;

    public bool IsNone => ReferenceEquals(value.Reference, NoneFlag);
    public bool BoolValue => value.Simple.BoolValue;
    public sbyte I8Value => value.Simple.I8Value;
    public byte ByteValue => value.Simple.ByteValue;
    public short I16Value => value.Simple.I16Value;
    public ushort U16Value => value.Simple.U16Value;
    public int I32Value => value.Simple.I32Value;
    public uint U32Value => value.Simple.U32Value;
    public long I64Value => value.Simple.I64Value;
    public ulong U64Value => value.Simple.U64Value;
    public nint OffsetValue => value.Simple.OffsetValue;
    public nuint SizeValue => value.Simple.SizeValue;

    #region Static Factory Methods/Properties
    public static readonly AzothValue None = new();

    public static AzothValue Object(AzothObject value) => new(value);
    public static AzothValue Int(BigInteger value) => new(value);
    public static AzothValue RawBoundedList(IRawBoundedList value) => new(value);
    public static AzothValue Promise(Task<AzothValue> value) => new(value);
    public static AzothValue FunctionReference(FunctionReference value) => new(value);
    public static AzothValue Bool(bool value) => new(value);
    public static AzothValue I8(sbyte value) => new(value);
    public static AzothValue Byte(byte value) => new(value);
    public static AzothValue I16(short value) => new(value);
    public static AzothValue U16(ushort value) => new(value);
    public static AzothValue I32(int value) => new(value);
    public static AzothValue U32(uint value) => new(value);
    public static AzothValue I64(long value) => new(value);
    public static AzothValue U64(ulong value) => new(value);
    public static AzothValue Offset(nint value) => new(value);
    public static AzothValue Size(nuint value) => new(value);
    #endregion

    #region Private Constructors
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AzothValue()
    {
        value.Reference = NoneFlag;
    }
    private AzothValue(AzothObject value)
    {
        ObjectValue = value;
    }
    private AzothValue(BigInteger value)
    {
        IntValue = value;
    }
    private AzothValue(IRawBoundedList value)
    {
        RawBoundedListValue = value;
    }
    private AzothValue(Task<AzothValue> value)
    {
        PromiseValue = value;
    }
    private AzothValue(FunctionReference value)
    {
        FunctionReferenceValue = value;
    }
    private AzothValue(bool value)
    {
        this.value.Simple.BoolValue = value;
    }
    private AzothValue(sbyte value)
    {
        this.value.Simple.I8Value = value;
    }
    private AzothValue(byte value)
    {
        this.value.Simple.ByteValue = value;
    }
    private AzothValue(short value)
    {
        this.value.Simple.I16Value = value;
    }
    private AzothValue(ushort value)
    {
        this.value.Simple.U16Value = value;
    }
    private AzothValue(int value)
    {
        this.value.Simple.I32Value = value;
    }
    private AzothValue(uint value)
    {
        this.value.Simple.U32Value = value;
    }
    private AzothValue(long value)
    {
        this.value.Simple.I64Value = value;
    }
    private AzothValue(ulong value)
    {
        this.value.Simple.U64Value = value;
    }

    private AzothValue(nint value)
    {
        this.value.Simple.OffsetValue = value;
    }
    private AzothValue(nuint value)
    {
        this.value.Simple.SizeValue = value;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

    public AzothValue Increment(ValueType numberType)
    {
        if (numberType == DataType.Int || numberType == DataType.UInt) return Int(IntValue + 1);
        if (numberType == DataType.Int8) return I8((sbyte)(I8Value + 1));
        if (numberType == DataType.Byte) return Byte((byte)(ByteValue + 1));
        if (numberType == DataType.Int16) return I16((short)(I16Value + 1));
        if (numberType == DataType.UInt16) return U16((ushort)(U16Value + 1));
        if (numberType == DataType.Int32) return I32(I32Value + 1);
        if (numberType == DataType.UInt32) return U32(U32Value + 1);
        if (numberType == DataType.Int64) return I64(I64Value + 1);
        if (numberType == DataType.UInt64) return U64(U64Value + 1);
        if (numberType == DataType.Size) return Size(SizeValue + 1);
        if (numberType == DataType.Offset) return Offset(OffsetValue + 1);

        throw new NotImplementedException($"Increment of {numberType}");
    }

    public AzothValue Decrement(ValueType numberType)
    {
        if (numberType == DataType.Int || numberType == DataType.UInt) return Int(IntValue - 1);
        if (numberType == DataType.Int8) return I8((sbyte)(I8Value - 1));
        if (numberType == DataType.Byte) return Byte((byte)(ByteValue - 1));
        if (numberType == DataType.Int16) return I16((short)(I16Value - 1));
        if (numberType == DataType.UInt16) return U16((ushort)(U16Value - 1));
        if (numberType == DataType.Int32) return I32(I32Value - 1);
        if (numberType == DataType.UInt32) return U32(U32Value - 1);
        if (numberType == DataType.Int64) return I64(I64Value - 1);
        if (numberType == DataType.UInt64) return U64(U64Value - 1);
        if (numberType == DataType.Size) return Size(SizeValue - 1);
        if (numberType == DataType.Offset) return Offset(OffsetValue - 1);

        throw new NotImplementedException($"Decrement of {numberType.ToILString()}");
    }

    public BigInteger ToBigInteger(ValueType numberType)
    {
        if (numberType == DataType.Int || numberType == DataType.UInt) return IntValue;
        if (numberType == DataType.Int8) return I8Value;
        if (numberType == DataType.Byte) return ByteValue;
        if (numberType == DataType.Int16) return I16Value;
        if (numberType == DataType.UInt16) return U16Value;
        if (numberType == DataType.Int32) return I32Value;
        if (numberType == DataType.UInt32) return U32Value;
        if (numberType == DataType.Int64) return I32Value;
        if (numberType == DataType.UInt64) return U32Value;
        if (numberType == DataType.Size) return SizeValue;
        if (numberType == DataType.Offset) return OffsetValue;

        throw new NotImplementedException($"ToBigInteger for {numberType.ToILString()}");
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct SimpleValue
    {
        [FieldOffset(0)] public bool BoolValue;
        [FieldOffset(0)] public sbyte I8Value;
        [FieldOffset(0)] public byte ByteValue;
        [FieldOffset(0)] public short I16Value;
        [FieldOffset(0)] public ushort U16Value;
        [FieldOffset(0)] public int I32Value;
        [FieldOffset(0)] public uint U32Value;
        [FieldOffset(0)] public long I64Value;
        [FieldOffset(0)] public ulong U64Value;
        //[FieldOffset(0)] public float F32Value;
        //[FieldOffset(0)] public double F64Value;
        [FieldOffset(0)] public nint OffsetValue;
        [FieldOffset(0)] public nuint SizeValue;
    }

    private struct SimpleValueType
    {
        public object? Reference;
        public SimpleValue Simple;
    }
}
