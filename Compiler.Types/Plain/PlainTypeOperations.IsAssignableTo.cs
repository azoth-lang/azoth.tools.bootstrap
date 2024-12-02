using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class PlainTypeOperations
{
    /// <summary>
    /// Whether a value of this plainType can be assigned to a variable of the other plainType. This
    /// accounts for both subtyping and implicit conversions.
    /// </summary>
    public static bool IsAssignableTo(this IMaybePlainType self, IMaybePlainType other)
    {
        if (self.IsSubtypeOf(other))
            return true;

        return (self, other) switch
        {
            (VoidPlainType, _) => false,
            (OptionalPlainType s, OptionalPlainType o)
                => s.Referent.IsAssignableTo(o.Referent),
            (_, OptionalPlainType o) => self.IsAssignableTo(o.Referent),
            (BoolLiteralTypeConstructor, BoolTypeConstructor) => true,
            (INumericPlainType s, INumericPlainType o) => s.IsImplicitlyNumericallyConvertibleTo(o),
            _ => false,
        };
    }

    private static bool IsImplicitlyNumericallyConvertibleTo(this INumericPlainType self, INumericPlainType other)
    {
        switch (self, other)
        {
            case (FixedSizeIntegerTypeConstructor s, FixedSizeIntegerTypeConstructor o):
                return o.Bits > s.Bits && (!s.IsSigned || o.IsSigned);
            case (IntegerLiteralTypeConstructor s, FixedSizeIntegerTypeConstructor o):
            {
                var requireSigned = s.IsSigned;
                var bits = s.Value.GetByteCount(!o.IsSigned) * 8;
                return o.Bits >= bits && (!requireSigned || o.IsSigned);
            }
            case (IntegerTypeConstructor, BigIntegerTypeConstructor { IsSigned: true }):
            case (IntegerTypeConstructor { IsSigned: false }, BigIntegerTypeConstructor):
            case (IntegerLiteralTypeConstructor, BigIntegerTypeConstructor { IsSigned: true }):
            case (IntegerLiteralTypeConstructor { IsSigned: false }, BigIntegerTypeConstructor):
                return true;
            case (IntegerLiteralTypeConstructor s, PointerSizedIntegerTypeConstructor o):
            {
                var requireSigned = s.IsSigned;
                return !requireSigned || o.IsSigned;
            }
            default:
                return false;
        }
    }
}
