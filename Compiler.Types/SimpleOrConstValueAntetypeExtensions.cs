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
            BoolAntetype _ => IType.Bool,
            BigIntegerAntetype t => t.IsSigned ? IType.Int : IType.UInt,
            PointerSizedIntegerAntetype t => t.ToType(),
            FixedSizeIntegerAntetype t => t.ToType(),
            _ => throw ExhaustiveMatch.Failed(antetype),
        };

    public static Type ToType(this PointerSizedIntegerAntetype antetype)
    {
        if (antetype.Equals((IMaybeExpressionAntetype)IAntetype.Size))
            return IType.Size;

        if (antetype.Equals((IMaybeExpressionAntetype)IAntetype.Offset))
            return IType.Offset;

        if (antetype.Equals((IMaybeExpressionAntetype)IAntetype.NInt))
            return IType.NInt;

        if (antetype.Equals((IMaybeExpressionAntetype)IAntetype.NUInt))
            return IType.NUInt;

        throw new UnreachableException();
    }

    public static Type ToType(this FixedSizeIntegerAntetype antetype)
        => antetype.Bits switch
        {
            8 => antetype.IsSigned ? IType.Int8 : IType.Byte,
            16 => antetype.IsSigned ? IType.Int16 : IType.UInt16,
            32 => antetype.IsSigned ? IType.Int32 : IType.UInt32,
            64 => antetype.IsSigned ? IType.Int64 : IType.UInt64,
            _ => throw new UnreachableException("Bits not an expected value"),
        };
}
