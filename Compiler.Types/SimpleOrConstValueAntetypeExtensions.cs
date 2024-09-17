using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static class SimpleOrConstValueAntetypeExtensions
{
    public static Type ToType(this ISimpleOrConstValueAntetype antetype)
        => antetype switch
        {
            ConstValueAntetype t => t.ToType(),
            SimpleAntetype t => t.ToType(),
            _ => throw ExhaustiveMatch.Failed(antetype),
        };

    public static Type ToType(this ConstValueAntetype antetype)
        => antetype switch
        {
            BoolConstValueAntetype t => (BoolConstValueType)t.Value,
            IntegerConstValueAntetype t => new IntegerConstValueType(t.Value),
            _ => throw ExhaustiveMatch.Failed(antetype),
        };

    public static Type ToType(this SimpleAntetype antetype)
        => antetype switch
        {
            BoolAntetype _ => DataType.Bool,
            BigIntegerAntetype t => t.IsSigned ? DataType.Int : DataType.UInt,
            PointerSizedIntegerAntetype t => t.ToType(),
            FixedSizeIntegerAntetype t => t.ToType(),
            _ => throw ExhaustiveMatch.Failed(antetype),
        };

    public static Type ToType(this PointerSizedIntegerAntetype antetype)
    {
        if (antetype.Equals((IMaybeExpressionAntetype)IAntetype.Size))
            return DataType.Size;

        if (antetype.Equals((IMaybeExpressionAntetype)IAntetype.Offset))
            return DataType.Offset;

        if (antetype.Equals((IMaybeExpressionAntetype)IAntetype.NInt))
            return DataType.NInt;

        if (antetype.Equals((IMaybeExpressionAntetype)IAntetype.NUInt))
            return DataType.NUInt;

        throw new UnreachableException();
    }

    public static Type ToType(this FixedSizeIntegerAntetype antetype)
        => antetype.Bits switch
        {
            8 => antetype.IsSigned ? DataType.Int8 : DataType.Byte,
            16 => antetype.IsSigned ? DataType.Int16 : DataType.UInt16,
            32 => antetype.IsSigned ? DataType.Int32 : DataType.UInt32,
            64 => antetype.IsSigned ? DataType.Int64 : DataType.UInt64,
            _ => throw new UnreachableException("Bits not an expected value"),
        };
}
