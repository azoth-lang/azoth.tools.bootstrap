using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class AntetypeOperations
{
    /// <summary>
    /// Whether a value of this antetype can be assigned to a variable of the other antetype. This
    /// accounts for both subtyping and implicit conversions.
    /// </summary>
    public static bool IsAssignableTo(this IMaybeExpressionAntetype self, IMaybeExpressionAntetype other)
    {
        if (self.IsSubtypeOf(other))
            return true;

        return (self, other) switch
        {
            (VoidAntetype, _) => false,
            (OptionalAntetype s, OptionalAntetype o)
                => s.Referent.IsAssignableTo(o.Referent),
            (_, OptionalAntetype o) => self.IsAssignableTo(o.Referent),
            (BoolLiteralTypeConstructor, BoolTypeConstructor) => true,
            (INumericAntetype s, INumericAntetype o) => s.IsImplicitlyNumericallyConvertibleTo(o),
            _ => false,
        };
    }

    private static bool IsImplicitlyNumericallyConvertibleTo(this INumericAntetype self, INumericAntetype other)
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
