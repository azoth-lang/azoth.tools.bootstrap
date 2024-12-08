using System;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;

/// <summary>
/// This is the type of an integer constant value, it isn't possible to declare a
/// variable to have this type.
/// </summary>
public sealed class IntegerConstValueType : ConstValueType
{
    public override bool IsTypeOfConstValue => true;
    public BigInteger Value { get; }
    public bool IsSigned { get; }

    public IntegerConstValueType(BigInteger value)
        : base(SpecialTypeName.ConstInt)
    {
        Value = value;
        IsSigned = value.Sign < 0;
    }

    public bool IsUInt16 => Value >= TypeConstructor.UInt16.MinValue && Value <= TypeConstructor.UInt16.MaxValue;
    public bool IsInt16 => Value >= TypeConstructor.Int16.MinValue && Value <= TypeConstructor.Int16.MaxValue;

    /// <summary>
    /// The default non-constant type to places values of this type in. For
    /// <see cref="IntegerConstValueType"/>, that is <see cref="IType.Int"/>.
    /// </summary>
    /// <remarks>It might be thought this should return the smallest integer type that contains
    /// the value. However, that would lead to unexpected behavior in some cases because small
    /// integer constants might produce small fixed size integers leading to overflow.</remarks>
    public override CapabilityType ToNonConstValueType() => IType.Int;

    public NumericTypeConstructor ToSmallestSignedIntegerType()
    {
        if (Value > TypeConstructor.Int64.MaxValue) return TypeConstructor.Int;
        if (Value > TypeConstructor.Int32.MaxValue) return TypeConstructor.Int64;
        if (Value > TypeConstructor.Int16.MaxValue) return TypeConstructor.Int32;
        if (Value > TypeConstructor.Int8.MaxValue) return TypeConstructor.Int16;
        if (Value < TypeConstructor.Int64.MinValue) return TypeConstructor.Int;
        if (Value < TypeConstructor.Int32.MinValue) return TypeConstructor.Int64;
        if (Value < TypeConstructor.Int16.MinValue) return TypeConstructor.Int32;
        if (Value < TypeConstructor.Int8.MinValue) return TypeConstructor.Int16;
        return TypeConstructor.Int8;
    }

    public NumericTypeConstructor ToSmallestUnsignedIntegerType()
    {
        if (IsSigned)
            throw new InvalidOperationException("Cannot convert signed value type to unsigned type.");
        if (Value > TypeConstructor.Int64.MaxValue) return TypeConstructor.UInt;
        if (Value > TypeConstructor.Int32.MaxValue) return TypeConstructor.UInt64;
        if (Value > TypeConstructor.Int16.MaxValue) return TypeConstructor.UInt32;
        if (Value > TypeConstructor.Byte.MaxValue) return TypeConstructor.UInt16;
        return TypeConstructor.Byte;
    }

    #region Equals
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is IntegerConstValueType otherType
               && Value == otherType.Value;
    }

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    public override INonVoidPlainType ToPlainType()
        => new IntegerLiteralTypeConstructor(Value).PlainType;

    public override Decorated.CapabilityType ToDecoratedType()
        => new(Capability.Constant, new(new IntegerLiteralTypeConstructor(Value), []));

    public override string ToSourceCodeString() => $"int[{Value}]";

    public override string ToILString() => $"int[{Value}]";
}
