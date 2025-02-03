using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout.BoundedLists;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

/// <summary>
/// A compact structure for representing Azoth values.
/// </summary>
/// <remarks><para>This struct is laid out in memory so that the first part is a reference and the
/// second is a primitive value. This corresponds with the internal layout of <see cref="BigInteger"/>
/// which stores the bit array in the first part and the <see cref="int"/> value second.</para>
///
/// <para>Since small value <see cref="BigInteger"/>s have a null reference, a special flag reference
/// must be used to indicate `none`.</para></remarks>
[StructLayout(LayoutKind.Explicit)]
internal readonly struct AzothValue
{
    public static AzothValue True = new(true);
    public static AzothValue False = new(false);

    private static readonly object NoneFlag = new();

    [FieldOffset(0)] public readonly AzothInstance InstanceValue;
    [FieldOffset(0)] public readonly AzothObject ObjectValue;
    [FieldOffset(0)] public readonly AzothStruct StructValue;
    [FieldOffset(0)] public readonly BigInteger IntValue;
    [FieldOffset(0)] public readonly IRawBoundedList RawBoundedListValue;
    [FieldOffset(0)] public readonly Task<AzothResult> PromiseValue;
    [FieldOffset(0)] public readonly FunctionReference FunctionReferenceValue;
    [FieldOffset(0)] private readonly SimpleValueType value;
    // This isn't used as a true value, this is only so AzothResult can contain arguments
    [FieldOffset(0)] public readonly List<AzothValue> ArgumentsValue;
    // These aren't used as true values, they are only so AzothInstance etc. can access fields
    [FieldOffset(0)] public readonly TypeLayout TypeLayoutValue;
    [FieldOffset(0)] public readonly VTable VTableValue;
    [FieldOffset(0)] public readonly StructLayout StructLayoutValue;

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
    public nint NIntValue => value.Simple.NIntValue;
    public nuint NUIntValue => value.Simple.NUIntValue;

    #region Static Factory Methods/Properties
    public static readonly AzothValue None = new();

    public static AzothValue Object(AzothObject value) => new(value);
    public static AzothValue Struct(AzothStruct value) => new(value);
    public static AzothValue Int(BigInteger value) => new(value);
    public static AzothValue RawBoundedList(IRawBoundedList value) => new(value);
    public static AzothValue Promise(Task<AzothResult> value) => new(value);
    public static AzothValue FunctionReference(FunctionReference value) => new(value);
    public static AzothValue Arguments(List<AzothValue> value) => new(value);
    public static AzothValue TypeLayout(TypeLayout typeLayout) => new(typeLayout);
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
    public static AzothValue NInt(nint value) => new(value);
    public static AzothValue NUInt(nuint value) => new(value);
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
    private AzothValue(AzothStruct value)
    {
        StructValue = value;
    }
    private AzothValue(BigInteger value)
    {
        IntValue = value;
    }
    private AzothValue(IRawBoundedList value)
    {
        RawBoundedListValue = value;
    }
    private AzothValue(Task<AzothResult> value)
    {
        PromiseValue = value;
    }
    private AzothValue(FunctionReference value)
    {
        FunctionReferenceValue = value;
    }
    private AzothValue(List<AzothValue> value)
    {
        ArgumentsValue = value;
    }
    private AzothValue(TypeLayout typeLayout)
    {
        TypeLayoutValue = typeLayout;
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
        [FieldOffset(0)] public nint NIntValue;
        [FieldOffset(0)] public nuint NUIntValue;
    }

    private struct SimpleValueType
    {
        public object? Reference;
        public SimpleValue Simple;
    }
}
