using System;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Declared;

/// <summary>
/// An integer represented in a fixed number of bits.
/// </summary>
public sealed class FixedSizeIntegerType : IntegerType
{
    internal new static readonly FixedSizeIntegerType Int8 = new(SpecialTypeName.Int8, -8);
    internal new static readonly FixedSizeIntegerType Byte = new(SpecialTypeName.Byte, 8);
    internal new static readonly FixedSizeIntegerType Int16 = new(SpecialTypeName.Int16, -16);
    internal new static readonly FixedSizeIntegerType UInt16 = new(SpecialTypeName.UInt16, 16);
    internal new static readonly FixedSizeIntegerType Int32 = new(SpecialTypeName.Int32, -32);
    internal new static readonly FixedSizeIntegerType UInt32 = new(SpecialTypeName.UInt32, 32);
    internal new static readonly FixedSizeIntegerType Int64 = new(SpecialTypeName.Int64, -64);
    internal new static readonly FixedSizeIntegerType UInt64 = new(SpecialTypeName.UInt64, 64);

    public int Bits { get; }
    public BigInteger MaxValue;
    public BigInteger MinValue;

    public override BareValueType<FixedSizeIntegerType> BareType { get; }

    public override ValueType<FixedSizeIntegerType> Type { get; }

    private FixedSizeIntegerType(SpecialTypeName name, int bits)
        : base(name, bits < 0)
    {
        Bits = Math.Abs(bits);
        if (IsSigned)
        {
            var powerOf2 = BigInteger.Pow(2, Bits - 1);
            MinValue = -powerOf2;
            MaxValue = powerOf2 - 1;
        }
        else
        {
            MinValue = 0;
            MaxValue = BigInteger.Pow(2, Bits);
        }
        BareType = new(this, FixedList<DataType>.Empty);
        Type = BareType.With(ReferenceCapability.Constant);
    }

    /// <summary>
    /// The current type but signed.
    /// </summary>
    /// <remarks>If the current type is already signed then this doesn't change anything. If the
    /// current type is unsigned, then this returns the next larger integer type.</remarks>
    public IntegerType WithSign()
    {
        if (IsSigned) return this;
        if (this == Byte) return Int32;
        return Int;
    }

    public override BareValueType<FixedSizeIntegerType> With(FixedList<DataType> typeArguments)
    {
        RequiresEmpty(typeArguments);
        return BareType;
    }

    public override ValueType<FixedSizeIntegerType> With(ReferenceCapability capability, FixedList<DataType> typeArguments)
        => With(typeArguments).With(capability);

    public override ValueType<FixedSizeIntegerType> With(ReferenceCapability capability)
        => BareType.With(capability);
}
