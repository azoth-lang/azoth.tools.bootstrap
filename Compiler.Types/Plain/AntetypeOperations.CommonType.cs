using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain.ConstValue;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class AntetypeOperations
{
    /// <summary>
    /// Determine what the common type for two numeric types for a numeric operator is.
    /// </summary>
    public static IAntetype? NumericOperatorCommonType(this IExpressionAntetype leftType, IExpressionAntetype rightType)
        => (leftType, rightType) switch
        {
            (_, NeverAntetype) => IAntetype.Never,
            (NeverAntetype, _) => IAntetype.Never,
            (OptionalAntetype { Referent: var left }, OptionalAntetype { Referent: var right })
                => left.NumericOperatorCommonType(right)?.MakeOptional(),
            (OptionalAntetype { Referent: var left }, _) => left.NumericOperatorCommonType(rightType)?.MakeOptional(),
            (_, OptionalAntetype { Referent: var right }) => leftType.NumericOperatorCommonType(right)?.MakeOptional(),
            (INumericAntetype left, INumericAntetype right)
                => left.NumericOperatorCommonType(right),
            _ => null,
        };

    /// <summary>
    /// Determine what the common type for two numeric types for a numeric operator is.
    /// </summary>
    internal static IAntetype? NumericOperatorCommonType(this INumericAntetype leftType, INumericAntetype rightType)
        => (leftType, rightType) switch
        {
            (BigIntegerTypeConstructor left, IntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (IntegerTypeConstructor left, BigIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (BigIntegerTypeConstructor left, IntegerConstValueAntetype right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (IntegerConstValueAntetype left, BigIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (PointerSizedIntegerTypeConstructor left, PointerSizedIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IAntetype.Offset : IAntetype.Size,
            (PointerSizedIntegerTypeConstructor { IsSigned: true }, IntegerConstValueAntetype { IsInt16: true })
                or (PointerSizedIntegerTypeConstructor { IsSigned: false }, IntegerConstValueAntetype { IsUInt16: true })
                => (IAntetype)leftType.Antetype,
            (PointerSizedIntegerTypeConstructor left, IntegerConstValueAntetype right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (IntegerConstValueAntetype { IsInt16: true }, PointerSizedIntegerTypeConstructor { IsSigned: true })
                or (IntegerConstValueAntetype { IsUInt16: true }, PointerSizedIntegerTypeConstructor { IsSigned: false })
                => (IAntetype)rightType.Antetype,
            (IntegerConstValueAntetype left, PointerSizedIntegerTypeConstructor right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (FixedSizeIntegerTypeConstructor left, FixedSizeIntegerTypeConstructor right)
                when left.IsSigned == right.IsSigned
                => left.Bits >= right.Bits ? left : right,
            (FixedSizeIntegerTypeConstructor { IsSigned: true } left, FixedSizeIntegerTypeConstructor right)
                when left.Bits > right.Bits
                => left,
            (FixedSizeIntegerTypeConstructor left, FixedSizeIntegerTypeConstructor { IsSigned: true } right)
                when left.Bits < right.Bits
                => right,
            (FixedSizeIntegerTypeConstructor { IsSigned: true } left, IntegerConstValueAntetype right)
                when left.IsSigned || right.IsSigned
                => left.NumericOperatorCommonType((INumericAntetype)right.ToSmallestSignedIntegerType()),
            (FixedSizeIntegerTypeConstructor { IsSigned: false } left, IntegerConstValueAntetype { IsSigned: false } right)
                => left.NumericOperatorCommonType((INumericAntetype)right.ToSmallestUnsignedIntegerType()),
            (IntegerConstValueAntetype left, FixedSizeIntegerTypeConstructor right)
                when left.IsSigned || right.IsSigned
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType((INumericAntetype)right),
            (IntegerConstValueAntetype { IsSigned: false } left, FixedSizeIntegerTypeConstructor { IsSigned: false } right)
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType((INumericAntetype)right),
            _ => null
        };
}
