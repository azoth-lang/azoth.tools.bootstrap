using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class PlainTypeOperations
{
    /// <summary>
    /// Whether a value of this <see cref="PlainType"/> can be assigned to a variable of the other
    /// <see cref="PlainType"/>. This accounts for both subtyping and implicit conversions.
    /// </summary>
    // TODO for this to work correctly, type constructors need to include implicit user defined conversions
    public static bool IsAssignableTo(this IMaybePlainType self, IMaybePlainType other)
        => self.IsSubtypeOf(other) || self.IsImplicitlyConvertibleTo(other);

    /// <summary>
    /// Whether a value of the <paramref name="other"/> <see cref="PlainType"/> can be assigned to
    /// a variable of <paramref name="self"/> <see cref="PlainType"/>. This accounts for both
    /// subtyping and implicit conversions.
    /// </summary>
    // TODO for this to work correctly, type constructors need to include implicit user defined conversions
    [Inline(export: true)]
    public static bool IsAssignableFrom(this IMaybePlainType self, IMaybePlainType other)
        => other.IsAssignableTo(self);

    // TODO what about the rules against multiple implicit conversions?
    /// <remarks>This is not publicly exposed because implicit conversions can inherently entail
    /// subtyping so the name doesn't quite make sense.</remarks>
    internal static bool IsImplicitlyConvertibleTo(this IMaybePlainType self, IMaybePlainType other)
    {
        return (self, other) switch
        {
            (VoidPlainType, _) => false,
            // While subtyping exists between optional types, this handles lifted implicit conversions
            (OptionalPlainType s, OptionalPlainType o)
                => s.Referent.IsAssignableTo(o.Referent),
            (PlainType s, OptionalPlainType o)
                => s.Semantics != TypeSemantics.Reference && s.IsAssignableTo(o.Referent),
            (BarePlainType { TypeConstructor: BoolLiteralTypeConstructor },
                BarePlainType { TypeConstructor: BoolTypeConstructor }) => true,
            (BarePlainType { TypeConstructor: SimpleOrLiteralTypeConstructor s },
                BarePlainType { TypeConstructor: SimpleOrLiteralTypeConstructor o })
                => s.IsImplicitlyNumericallyConvertibleTo(o),
            (RefPlainType s, _)
                => s.Referent.IsAssignableTo(other),
            _ => false,
        };
    }

    /// <remarks>This is not publicly exposed because of the strange handling of literal types. But
    /// once that is handled, it might make sense to make this method public.</remarks>
    private static bool IsImplicitlyNumericallyConvertibleTo(
        this SimpleOrLiteralTypeConstructor self,
        SimpleOrLiteralTypeConstructor other)
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
