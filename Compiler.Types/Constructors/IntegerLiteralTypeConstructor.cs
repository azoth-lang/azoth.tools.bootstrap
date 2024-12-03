using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

// TODO this isn't right. The value should be a type parameter
public sealed class IntegerLiteralTypeConstructor : LiteralTypeConstructor, INumericTypeConstructor
{
    public BigInteger Value { get; }
    public bool IsSigned => Value.Sign < 0;

    public bool IsUInt16
        => Value >= ITypeConstructor.UInt16.MinValue && Value <= ITypeConstructor.UInt16.MaxValue;
    public bool IsInt16
        => Value >= ITypeConstructor.Int16.MinValue && Value <= ITypeConstructor.Int16.MaxValue;

    public override OrdinaryNamedPlainType PlainType => LazyInitializer.EnsureInitialized(ref plainType, ConstructPlainType);
    private OrdinaryNamedPlainType? plainType;

    public IntegerLiteralTypeConstructor(BigInteger value)
        : base(SpecialTypeName.Int)
    {
        Value = value;
    }

    /// <remarks>For <see cref="IntegerLiteralTypeConstructor"/>, this is
    /// <see cref="ITypeConstructor.Int"/>. It might be thought this should return the smallest
    /// integer type constructor that contains the value. However, that would lead to unexpected
    /// behavior in some cases because small integer constants might produce small fixed size
    /// integers leading to overflow.</remarks>
    public override ITypeConstructor ToNonLiteral() => ITypeConstructor.Int;

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
    public override bool Equals(ITypeConstructor? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is IntegerLiteralTypeConstructor that
               && Value.Equals(that.Value);
    }

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    private OrdinaryNamedPlainType ConstructPlainType() => new(this, []);

    public override string ToString() => $"int[{Value}]";
}
