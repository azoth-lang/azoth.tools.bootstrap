using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using InlineMethod;

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
internal readonly struct Value
{
    [FieldOffset(0)] public readonly InstanceReference InstanceReference;
    [FieldOffset(0)] public readonly ClassReference ClassReference;
    [FieldOffset(0)] public readonly StructReference StructReference;
    [FieldOffset(0)] public readonly ValueReference ValueReference;
    [FieldOffset(0)] public readonly BigInteger Int;
    [FieldOffset(0)] public readonly IIntrinsicValue IntrinsicValue;
    [FieldOffset(0)] public readonly Task<Result> Promise;
    [FieldOffset(0)] public readonly FunctionReference FunctionReference;
    [FieldOffset(0)] private readonly SimpleValueType value;
    // This isn't used as a true value, this is only so Result can contain arguments
    [FieldOffset(0)] public readonly List<Value> Arguments;
    // These aren't used as true values, they are only so InstanceReference etc. can access fields
    [FieldOffset(0)] public readonly TypeMetadata TypeMetadata;
    [FieldOffset(0)] public readonly ClassMetadata ClassMetadata;
    [FieldOffset(0)] public readonly StructMetadata StructMetadata;
    [FieldOffset(0)] public readonly ValueMetadata ValueMetadata;
    [FieldOffset(0)] public readonly BareType BareType;

    public bool IsNone
    {
        [Inline(export: true)]
        get => ReferenceEquals(value.Reference, NoneFlag.Instance);
    }

    public bool Bool => value.Simple.Bool;
    public sbyte I8 => value.Simple.I8;
    public byte Byte => value.Simple.Byte;
    public short I16 => value.Simple.I16;
    public ushort U16 => value.Simple.U16;
    public int I32 => value.Simple.I32;
    public uint U32 => value.Simple.U32;
    public long I64 => value.Simple.I64;
    public ulong U64 => value.Simple.U64;
    public nint Offset => value.Simple.Offset;
    public nuint Size => value.Simple.Size;
    public nint NInt => value.Simple.NInt;
    public nuint NUInt => value.Simple.NUInt;

    #region Static Factory Methods/Properties
    public static Value True = new(true);
    public static Value False = new(false);
    public static readonly Value None = new(NoneFlag.Instance);

    [Inline(InlineBehavior.Remove)]
    public static Value From(ClassReference value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value From(StructReference value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value From(ValueReference value) => new(value);
    [Inline(export: true)]
    public static Value From(BigInteger value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value From(IIntrinsicValue value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value From(Task<Result> value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value From(FunctionReference value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromArguments(List<Value> value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value From(TypeMetadata typeMetadata) => new(typeMetadata);
    [Inline(InlineBehavior.Remove)]
    public static Value From(BareType bareType) => new(bareType);
    [Inline(InlineBehavior.Remove)]
    public static Value From(bool value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromI8(sbyte value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromByte(byte value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromI16(short value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromU16(ushort value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromI32(int value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromU32(uint value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromI64(long value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromU64(ulong value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromOffset(nint value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromSize(nuint value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromNInt(nint value) => new(value);
    [Inline(InlineBehavior.Remove)]
    public static Value FromNUInt(nuint value) => new(value);
    #endregion

    #region Private Constructors
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Value(NoneFlag value)
    {
        this.value.Reference = value;
    }
    private Value(ClassReference value)
    {
        ClassReference = value;
    }
    private Value(StructReference value)
    {
        StructReference = value;
    }
    private Value(ValueReference value)
    {
        ValueReference = value;
    }
    private Value(BigInteger value)
    {
        Int = value;
    }
    private Value(IIntrinsicValue value)
    {
        IntrinsicValue = value;
    }
    private Value(Task<Result> value)
    {
        Promise = value;
    }
    private Value(FunctionReference value)
    {
        FunctionReference = value;
    }
    private Value(List<Value> value)
    {
        Arguments = value;
    }
    private Value(TypeMetadata typeMetadata)
    {
        TypeMetadata = typeMetadata;
    }
    private Value(BareType bareType)
    {
        BareType = bareType;
    }
    private Value(bool value)
    {
        this.value.Simple.Bool = value;
    }
    private Value(sbyte value)
    {
        this.value.Simple.I8 = value;
    }
    private Value(byte value)
    {
        this.value.Simple.Byte = value;
    }
    private Value(short value)
    {
        this.value.Simple.I16 = value;
    }
    private Value(ushort value)
    {
        this.value.Simple.U16 = value;
    }
    private Value(int value)
    {
        this.value.Simple.I32 = value;
    }
    private Value(uint value)
    {
        this.value.Simple.U32 = value;
    }
    private Value(long value)
    {
        this.value.Simple.I64 = value;
    }
    private Value(ulong value)
    {
        this.value.Simple.U64 = value;
    }
    private Value(nint value)
    {
        this.value.Simple.Offset = value;
    }
    private Value(nuint value)
    {
        this.value.Simple.Size = value;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

    [Inline(InlineBehavior.Remove)]
    public FunctionReference? AsFunctionReference() => value.Reference as FunctionReference;

    [StructLayout(LayoutKind.Explicit)]
    private struct SimpleValue
    {
        [FieldOffset(0)] public bool Bool;
        [FieldOffset(0)] public sbyte I8;
        [FieldOffset(0)] public byte Byte;
        [FieldOffset(0)] public short I16;
        [FieldOffset(0)] public ushort U16;
        [FieldOffset(0)] public int I32;
        [FieldOffset(0)] public uint U32;
        [FieldOffset(0)] public long I64;
        [FieldOffset(0)] public ulong U64;
        [FieldOffset(0)] public Half F16;
        [FieldOffset(0)] public float F32;
        [FieldOffset(0)] public double F64;
        [FieldOffset(0)] public nint Offset;
        [FieldOffset(0)] public nuint Size;
        [FieldOffset(0)] public nint NInt;
        [FieldOffset(0)] public nuint NUInt;
    }

    private struct SimpleValueType
    {
        public object? Reference;
        public SimpleValue Simple;
    }
}
