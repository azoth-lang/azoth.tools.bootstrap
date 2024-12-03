using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public static class SimpleOrLiteralTypeConstructorExtensions
{
    public static IExpressionType ToType(this OrdinaryNamedPlainType plainType)
        => plainType.TypeConstructor switch
        {
            SimpleOrLiteralTypeConstructor t => t.ToType(),
            AnyTypeConstructor _
                => throw new NotSupportedException("Conversion to type only supported on simple or literal type constructor"),
            OrdinaryTypeConstructor _
                => throw new NotSupportedException("Conversion to type only supported on simple or literal type constructor"),
            _ => throw ExhaustiveMatch.Failed(plainType.TypeConstructor),
        };

    public static IExpressionType ToType(this SimpleOrLiteralTypeConstructor typeConstructor)
        => typeConstructor switch
        {
            LiteralTypeConstructor t => t.ToType(),
            SimpleTypeConstructor t => t.ToType(),
            _ => throw ExhaustiveMatch.Failed(typeConstructor),
        };

    public static ConstValueType ToType(this LiteralTypeConstructor typeConstructor)
        => typeConstructor switch
        {
            BoolLiteralTypeConstructor t => (BoolConstValueType)t.Value,
            IntegerLiteralTypeConstructor t => new IntegerConstValueType(t.Value),
            _ => throw ExhaustiveMatch.Failed(typeConstructor),
        };

    public static CapabilityType ToType(this SimpleTypeConstructor typeConstructor)
        => typeConstructor switch
        {
            BoolTypeConstructor _ => IType.Bool,
            BigIntegerTypeConstructor t => t.IsSigned ? IType.Int : IType.UInt,
            PointerSizedIntegerTypeConstructor t => t.ToType(),
            FixedSizeIntegerTypeConstructor t => t.ToType(),
            _ => throw ExhaustiveMatch.Failed(typeConstructor),
        };

    public static CapabilityType<PointerSizedIntegerType> ToType(this PointerSizedIntegerTypeConstructor typeConstructor)
    {
        if (typeConstructor.Equals(TypeConstructor.Size))
            return IType.Size;

        if (typeConstructor.Equals(TypeConstructor.Offset))
            return IType.Offset;

        if (typeConstructor.Equals(TypeConstructor.NInt))
            return IType.NInt;

        if (typeConstructor.Equals(TypeConstructor.NUInt))
            return IType.NUInt;

        throw new UnreachableException();
    }

    public static CapabilityType<FixedSizeIntegerType> ToType(this FixedSizeIntegerTypeConstructor typeConstructor)
        => typeConstructor.Bits switch
        {
            8 => typeConstructor.IsSigned ? IType.Int8 : IType.Byte,
            16 => typeConstructor.IsSigned ? IType.Int16 : IType.UInt16,
            32 => typeConstructor.IsSigned ? IType.Int32 : IType.UInt32,
            64 => typeConstructor.IsSigned ? IType.Int64 : IType.UInt64,
            _ => throw new UnreachableException("Bits not an expected value"),
        };
}
