using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class PlainTypeOperations
{
    /// <summary>
    /// Determine what the common type for two numeric types for a numeric operator is.
    /// </summary>
    public static PlainType? NumericOperatorCommonType(this PlainType leftType, PlainType rightType)
        => (leftType, rightType) switch
        {
            (_, NeverPlainType) => PlainType.Never,
            (NeverPlainType, _) => PlainType.Never,
            (OptionalPlainType { Referent: var left }, OptionalPlainType { Referent: var right })
                => left.NumericOperatorCommonType(right)?.MakeOptional(),
            (OptionalPlainType { Referent: var left }, _) => left.NumericOperatorCommonType(rightType)?.MakeOptional(),
            (_, OptionalPlainType { Referent: var right }) => leftType.NumericOperatorCommonType(right)?.MakeOptional(),
            (ConstructedPlainType { TypeConstructor: SimpleOrLiteralTypeConstructor left },
                ConstructedPlainType { TypeConstructor: SimpleOrLiteralTypeConstructor right })
                => left.NumericOperatorCommonType(right),
            _ => null,
        };

    /// <summary>
    /// Determine what the common type for two numeric types for a numeric operator is.
    /// </summary>
    internal static PlainType? NumericOperatorCommonType(
        this SimpleOrLiteralTypeConstructor leftTypeConstructor,
        SimpleOrLiteralTypeConstructor rightTypeConstructor)
        => (leftType: leftTypeConstructor, rightType: rightTypeConstructor) switch
        {
            (BigIntegerTypeConstructor left, IntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? PlainType.Int : PlainType.UInt,
            (IntegerTypeConstructor left, BigIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? PlainType.Int : PlainType.UInt,
            (BigIntegerTypeConstructor left, IntegerLiteralTypeConstructor right)
                => left.IsSigned || right.IsSigned ? PlainType.Int : PlainType.UInt,
            (IntegerLiteralTypeConstructor left, BigIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? PlainType.Int : PlainType.UInt,
            (PointerSizedIntegerTypeConstructor left, PointerSizedIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? PlainType.Offset : PlainType.Size,
            (PointerSizedIntegerTypeConstructor { IsSigned: true }, IntegerLiteralTypeConstructor { IsInt16: true })
                or (PointerSizedIntegerTypeConstructor { IsSigned: false }, IntegerLiteralTypeConstructor { IsUInt16: true })
                => leftTypeConstructor.PlainType,
            (PointerSizedIntegerTypeConstructor left, IntegerLiteralTypeConstructor right)
                => left.IsSigned || right.IsSigned ? PlainType.Int : PlainType.UInt,
            (IntegerLiteralTypeConstructor { IsInt16: true }, PointerSizedIntegerTypeConstructor { IsSigned: true })
                or (IntegerLiteralTypeConstructor { IsUInt16: true }, PointerSizedIntegerTypeConstructor { IsSigned: false })
                => rightTypeConstructor.PlainType,
            (IntegerLiteralTypeConstructor left, PointerSizedIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? PlainType.Int : PlainType.UInt,
            (FixedSizeIntegerTypeConstructor left, FixedSizeIntegerTypeConstructor right)
                when left.IsSigned == right.IsSigned
                => left.Bits >= right.Bits ? left.PlainType : right.PlainType,
            (FixedSizeIntegerTypeConstructor { IsSigned: true } left, FixedSizeIntegerTypeConstructor right)
                when left.Bits > right.Bits
                => left.PlainType,
            (FixedSizeIntegerTypeConstructor left, FixedSizeIntegerTypeConstructor { IsSigned: true } right)
                when left.Bits < right.Bits
                => right.PlainType,
            (FixedSizeIntegerTypeConstructor { IsSigned: true } left, IntegerLiteralTypeConstructor right)
                when left.IsSigned || right.IsSigned
                => left.NumericOperatorCommonType(right.ToSmallestSignedIntegerType()),
            (FixedSizeIntegerTypeConstructor { IsSigned: false } left, IntegerLiteralTypeConstructor { IsSigned: false } right)
                => left.NumericOperatorCommonType(right.ToSmallestUnsignedIntegerType()),
            (IntegerLiteralTypeConstructor left, FixedSizeIntegerTypeConstructor right)
                when left.IsSigned || right.IsSigned
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType(right),
            (IntegerLiteralTypeConstructor { IsSigned: false } left, FixedSizeIntegerTypeConstructor { IsSigned: false } right)
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType(right),
            _ => null
        };
}
