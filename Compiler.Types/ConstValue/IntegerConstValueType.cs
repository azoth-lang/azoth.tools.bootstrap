using System;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;

/// <summary>
/// This is the type of an integer constant value, it isn't possible to declare a
/// variable to have this type.
/// </summary>
public sealed class IntegerConstValueType : ConstValueType, INumericType
{
    public override bool IsTypeOfConstValue => true;
    public BigInteger Value { get; }
    public bool IsSigned { get; }
    public override bool IsFullyKnown => true;
    DataType INumericType.Type => this;

    public IntegerConstValueType(BigInteger value)
        : base(SpecialTypeName.ConstInt)
    {
        Value = value;
        IsSigned = value.Sign < 0;
    }

    public bool IsUInt16 => Value >= DeclaredType.UInt16.MinValue && Value <= DeclaredType.UInt16.MaxValue;
    public bool IsInt16 => Value >= DeclaredType.Int16.MinValue && Value <= DeclaredType.Int16.MaxValue;

    public IntegerConstValueType Add(IntegerConstValueType right) => new(Value + right.Value);
    public IntegerConstValueType Subtract(IntegerConstValueType right) => new(Value - right.Value);
    public IntegerConstValueType Multiply(IntegerConstValueType right) => new(Value * right.Value);
    public IntegerConstValueType DivideBy(IntegerConstValueType right) => new(Value / right.Value);
    public IntegerConstValueType Negate() => new(-Value);
    public BoolConstValueType Equals(IntegerConstValueType right) => Value == right.Value;
    public BoolConstValueType NotEquals(IntegerConstValueType right) => Value != right.Value;
    public BoolConstValueType LessThan(IntegerConstValueType right) => Value < right.Value;
    public BoolConstValueType LessThanOrEqual(IntegerConstValueType right) => Value <= right.Value;
    public BoolConstValueType GreaterThan(IntegerConstValueType right) => Value > right.Value;
    public BoolConstValueType GreaterThanOrEqual(IntegerConstValueType right) => Value >= right.Value;

    /// <summary>
    /// The default non-constant type to places values of this type in. For
    /// <see cref="IntegerConstValueType"/>, that is <see cref="DataType.Int"/>.
    /// </summary>
    /// <remarks>It might be thought this should return the smallest integer type that contains
    /// the value. However, that would lead to unexpected behavior in some cases because small
    /// integer constants might produce small fixed size integers leading to overflow.</remarks>
    public override DataType ToNonConstantType() => Int;

    public NumericType ToSmallestSignedIntegerType()
    {
        if (Value > DeclaredType.Int64.MaxValue) return DeclaredType.Int;
        if (Value > DeclaredType.Int32.MaxValue) return DeclaredType.Int64;
        if (Value > DeclaredType.Int16.MaxValue) return DeclaredType.Int32;
        if (Value > DeclaredType.Int8.MaxValue) return DeclaredType.Int16;
        if (Value < DeclaredType.Int64.MinValue) return DeclaredType.Int;
        if (Value < DeclaredType.Int32.MinValue) return DeclaredType.Int64;
        if (Value < DeclaredType.Int16.MinValue) return DeclaredType.Int32;
        if (Value < DeclaredType.Int8.MinValue) return DeclaredType.Int16;
        return DeclaredType.Int8;
    }

    public NumericType ToSmallestUnsignedIntegerType()
    {
        if (IsSigned)
            throw new InvalidOperationException("Cannot convert signed value type to unsigned type.");
        if (Value > DeclaredType.Int64.MaxValue) return DeclaredType.UInt;
        if (Value > DeclaredType.Int32.MaxValue) return DeclaredType.UInt64;
        if (Value > DeclaredType.Int16.MaxValue) return DeclaredType.UInt32;
        if (Value > DeclaredType.Byte.MaxValue) return DeclaredType.UInt16;
        return DeclaredType.Byte;
    }

    #region Equals
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is IntegerConstValueType otherType
               && Value == otherType.Value;
    }

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    public override IMaybeExpressionAntetype ToAntetype() => new IntegerConstValueAntetype(Value);

    public override string ToSourceCodeString()
        => throw new InvalidOperationException("Integer value type has no source code representation");

    public override string ToILString() => $"Value[{Value}]";
}
