using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public static class SimpleOrConstValueAntetypeExtensions
{
    public static IExpressionType ToType(this ISimpleOrConstValueAntetype antetype)
        => antetype switch
        {
            LiteralTypeConstructor t => t.ToType(),
            SimpleTypeConstructor t => t.ToType(),
            _ => throw ExhaustiveMatch.Failed(antetype),
        };

    public static ConstValueType ToType(this LiteralTypeConstructor antetype)
        => antetype switch
        {
            BoolLiteralTypeConstructor t => (BoolConstValueType)t.Value,
            IntegerLiteralTypeConstructor t => new IntegerConstValueType(t.Value),
            _ => throw ExhaustiveMatch.Failed(antetype),
        };

    public static CapabilityType ToType(this SimpleTypeConstructor antetype)
        => antetype switch
        {
            BoolTypeConstructor _ => IType.Bool,
            BigIntegerTypeConstructor t => t.IsSigned ? IType.Int : IType.UInt,
            PointerSizedIntegerTypeConstructor t => t.ToType(),
            FixedSizeIntegerTypeConstructor t => t.ToType(),
            _ => throw ExhaustiveMatch.Failed(antetype),
        };

    public static CapabilityType<PointerSizedIntegerType> ToType(this PointerSizedIntegerTypeConstructor typeConstructor)
    {
        if (typeConstructor.Equals((IMaybePlainType)ITypeConstructor.Size))
            return IType.Size;

        if (typeConstructor.Equals((IMaybePlainType)ITypeConstructor.Offset))
            return IType.Offset;

        if (typeConstructor.Equals((IMaybePlainType)ITypeConstructor.NInt))
            return IType.NInt;

        if (typeConstructor.Equals((IMaybePlainType)ITypeConstructor.NUInt))
            return IType.NUInt;

        throw new UnreachableException();
    }

    public static CapabilityType<FixedSizeIntegerType> ToType(this FixedSizeIntegerTypeConstructor antetype)
        => antetype.Bits switch
        {
            8 => antetype.IsSigned ? IType.Int8 : IType.Byte,
            16 => antetype.IsSigned ? IType.Int16 : IType.UInt16,
            32 => antetype.IsSigned ? IType.Int32 : IType.UInt32,
            64 => antetype.IsSigned ? IType.Int64 : IType.UInt64,
            _ => throw new UnreachableException("Bits not an expected value"),
        };
}
