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
            (BigIntegerAntetype left, IntegerAntetype right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (IntegerAntetype left, BigIntegerAntetype right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (BigIntegerAntetype left, IntegerConstValueAntetype right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (IntegerConstValueAntetype left, BigIntegerAntetype right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (PointerSizedIntegerAntetype left, PointerSizedIntegerAntetype right)
                => left.IsSigned || right.IsSigned ? IAntetype.Offset : IAntetype.Size,
            (PointerSizedIntegerAntetype { IsSigned: true }, IntegerConstValueAntetype { IsInt16: true })
                or (PointerSizedIntegerAntetype { IsSigned: false }, IntegerConstValueAntetype { IsUInt16: true })
                => (IAntetype)leftType.Antetype,
            (PointerSizedIntegerAntetype left, IntegerConstValueAntetype right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (IntegerConstValueAntetype { IsInt16: true }, PointerSizedIntegerAntetype { IsSigned: true })
                or (IntegerConstValueAntetype { IsUInt16: true }, PointerSizedIntegerAntetype { IsSigned: false })
                => (IAntetype)rightType.Antetype,
            (IntegerConstValueAntetype left, PointerSizedIntegerAntetype right)
                => left.IsSigned || right.IsSigned ? IAntetype.Int : IAntetype.UInt,
            (FixedSizeIntegerAntetype left, FixedSizeIntegerAntetype right)
                when left.IsSigned == right.IsSigned
                => left.Bits >= right.Bits ? left : right,
            (FixedSizeIntegerAntetype { IsSigned: true } left, FixedSizeIntegerAntetype right)
                when left.Bits > right.Bits
                => left,
            (FixedSizeIntegerAntetype left, FixedSizeIntegerAntetype { IsSigned: true } right)
                when left.Bits < right.Bits
                => right,
            (FixedSizeIntegerAntetype { IsSigned: true } left, IntegerConstValueAntetype right)
                when left.IsSigned || right.IsSigned
                => left.NumericOperatorCommonType((INumericAntetype)right.ToSmallestSignedIntegerType()),
            (FixedSizeIntegerAntetype { IsSigned: false } left, IntegerConstValueAntetype { IsSigned: false } right)
                => left.NumericOperatorCommonType((INumericAntetype)right.ToSmallestUnsignedIntegerType()),
            (IntegerConstValueAntetype left, FixedSizeIntegerAntetype right)
                when left.IsSigned || right.IsSigned
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType((INumericAntetype)right),
            (IntegerConstValueAntetype { IsSigned: false } left, FixedSizeIntegerAntetype { IsSigned: false } right)
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType((INumericAntetype)right),
            _ => null
        };
}
