using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public sealed class IntegerLiteralTypeConstructor : LiteralTypeConstructor, INumericAntetype
{
    public BigInteger Value { get; }
    public bool IsSigned => Value.Sign < 0;
    IExpressionAntetype INumericAntetype.Antetype => this;

    public bool IsUInt16
        => Value >= ITypeConstructor.UInt16.MinValue && Value <= ITypeConstructor.UInt16.MaxValue;
    public bool IsInt16
        => Value >= ITypeConstructor.Int16.MinValue && Value <= ITypeConstructor.Int16.MaxValue;

    public IntegerLiteralTypeConstructor(BigInteger value)
        : base(SpecialTypeName.ConstInt)
    {
        Value = value;
    }

    /// <summary>
    /// The default non-constant type to places values of this type in. For
    /// <see cref="IntegerLiteralTypeConstructor"/>, that is <see cref="IAntetype.Int"/>.
    /// </summary>
    /// <remarks>It might be thought this should return the smallest integer type that contains
    /// the value. However, that would lead to unexpected behavior in some cases because small
    /// integer constants might produce small fixed size integers leading to overflow.</remarks>
    public override IAntetype ToNonConstValueType() => ITypeConstructor.Int;

    public NumericTypeConstructor ToSmallestSignedIntegerType()
    {
        if (Value > ITypeConstructor.Int64.MaxValue) return ITypeConstructor.Int;
        if (Value > ITypeConstructor.Int32.MaxValue) return ITypeConstructor.Int64;
        if (Value > ITypeConstructor.Int16.MaxValue) return ITypeConstructor.Int32;
        if (Value > ITypeConstructor.Int8.MaxValue) return ITypeConstructor.Int16;
        if (Value < ITypeConstructor.Int64.MinValue) return ITypeConstructor.Int;
        if (Value < ITypeConstructor.Int32.MinValue) return ITypeConstructor.Int64;
        if (Value < ITypeConstructor.Int16.MinValue) return ITypeConstructor.Int32;
        if (Value < ITypeConstructor.Int8.MinValue) return ITypeConstructor.Int16;
        return ITypeConstructor.Int8;
    }

    public NumericTypeConstructor ToSmallestUnsignedIntegerType()
    {
        if (IsSigned) throw new InvalidOperationException("Cannot convert signed value type to unsigned type.");
        if (Value > ITypeConstructor.Int64.MaxValue) return ITypeConstructor.UInt;
        if (Value > ITypeConstructor.Int32.MaxValue) return ITypeConstructor.UInt64;
        if (Value > ITypeConstructor.Int16.MaxValue) return ITypeConstructor.UInt32;
        if (Value > ITypeConstructor.Byte.MaxValue) return ITypeConstructor.UInt16;
        return ITypeConstructor.Byte;
    }

    #region Operations
    public IntegerLiteralTypeConstructor Add(IntegerLiteralTypeConstructor other)
        => new(Value + other.Value);

    public IntegerLiteralTypeConstructor Subtract(IntegerLiteralTypeConstructor other)
        => new(Value - other.Value);

    public IntegerLiteralTypeConstructor Multiply(IntegerLiteralTypeConstructor other)
        => new(Value * other.Value);

    public IntegerLiteralTypeConstructor DivideBy(IntegerLiteralTypeConstructor other)
        => new(Value / other.Value);

    public IntegerLiteralTypeConstructor Negate() => new(-Value);

    public BoolLiteralTypeConstructor Equals(IntegerLiteralTypeConstructor other)
        => Value == other.Value;

    public BoolLiteralTypeConstructor NotEquals(IntegerLiteralTypeConstructor other)
        => Value != other.Value;

    public BoolLiteralTypeConstructor LessThan(IntegerLiteralTypeConstructor other)
        => Value < other.Value;

    public BoolLiteralTypeConstructor LessThanOrEqual(IntegerLiteralTypeConstructor other)
        => Value <= other.Value;

    public BoolLiteralTypeConstructor GreaterThan(IntegerLiteralTypeConstructor other)
        => Value > other.Value;

    public BoolLiteralTypeConstructor GreaterThanOrEqual(IntegerLiteralTypeConstructor other)
        => Value >= other.Value;
    #endregion

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is IntegerLiteralTypeConstructor that
               && Value.Equals(that.Value);
    }

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    public override string ToString() => $"int[{Value}]";
}
