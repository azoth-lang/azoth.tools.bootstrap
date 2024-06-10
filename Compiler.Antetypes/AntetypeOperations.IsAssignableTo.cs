using Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

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
            (BoolConstValueAntetype, BoolAntetype) => true,
            (INumericAntetype s, INumericAntetype o) => s.IsImplicitlyNumericallyConvertibleTo(o),
            _ => false,
        };
    }

    private static bool IsImplicitlyNumericallyConvertibleTo(this INumericAntetype self, INumericAntetype other)
    {
        switch (self, other)
        {
            case (FixedSizeIntegerAntetype s, FixedSizeIntegerAntetype o):
                return o.Bits > s.Bits && (!s.IsSigned || o.IsSigned);
            case (IntegerConstValueAntetype s, FixedSizeIntegerAntetype o):
            {
                var requireSigned = s.IsSigned;
                var bits = s.Value.GetByteCount(!o.IsSigned) * 8;
                return o.Bits >= bits && (!requireSigned || o.IsSigned);
            }
            case (IntegerAntetype, BigIntegerAntetype { IsSigned: true }):
            case (IntegerAntetype { IsSigned: false }, BigIntegerAntetype):
            case (IntegerConstValueAntetype, BigIntegerAntetype { IsSigned: true }):
            case (IntegerConstValueAntetype { IsSigned: false }, BigIntegerAntetype):
                return true;
            case (IntegerConstValueAntetype s, PointerSizedIntegerAntetype o):
            {
                var requireSigned = s.IsSigned;
                return !requireSigned || o.IsSigned;
            }
            default:
                return false;
        }
    }
}
