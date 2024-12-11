using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

// TODO this isn't right. The value should be a type parameter
public sealed class IntegerLiteralTypeConstructor : LiteralTypeConstructor
{
    public BigInteger Value { get; }
    public bool IsSigned => Value.Sign < 0;

    public bool IsUInt16
        => Value >= UInt16.MinValue && Value <= UInt16.MaxValue;
    public bool IsInt16
        => Value >= Int16.MinValue && Value <= Int16.MaxValue;

    public override ConstructedPlainType PlainType
        => LazyInitializer.EnsureInitialized(ref plainType, ConstructPlainType);
    private ConstructedPlainType? plainType;

    public IntegerLiteralTypeConstructor(BigInteger value)
        : base(SpecialTypeName.Int)
    {
        Value = value;
    }

    /// <remarks>For <see cref="IntegerLiteralTypeConstructor"/>, this is
    /// <see cref="TypeConstructor.Int"/>. It might be thought this should return the smallest
    /// integer type constructor that contains the value. However, that would lead to unexpected
    /// behavior in some cases because small integer constants might produce small fixed size
    /// integers leading to overflow.</remarks>
    public override TypeConstructor ToNonLiteral() => Int;

    public IntegerTypeConstructor ToSmallestSignedIntegerType()
    {
        if (Value > Int64.MaxValue) return Int;
        if (Value > Int32.MaxValue) return Int64;
        if (Value > Int16.MaxValue) return Int32;
        if (Value > Int8.MaxValue) return Int16;
        if (Value < Int64.MinValue) return Int;
        if (Value < Int32.MinValue) return Int64;
        if (Value < Int16.MinValue) return Int32;
        if (Value < Int8.MinValue) return Int16;
        return Int8;
    }

    public IntegerTypeConstructor ToSmallestUnsignedIntegerType()
    {
        if (IsSigned) throw new InvalidOperationException("Cannot convert signed value type to unsigned type.");
        if (Value > Int64.MaxValue) return UInt;
        if (Value > Int32.MaxValue) return UInt64;
        if (Value > Int16.MaxValue) return UInt32;
        if (Value > Byte.MaxValue) return UInt16;
        return Byte;
    }

    // TODO remove operations except for negate. Literal types should not be used for constant folding `1 + 1` doesn't have the type `int[2]`
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
    public override bool Equals(TypeConstructor? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is IntegerLiteralTypeConstructor that
               && Value.Equals(that.Value);
    }

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    private ConstructedPlainType ConstructPlainType() => new(this, []);

    public override string ToString() => $"int[{Value}]";
}
