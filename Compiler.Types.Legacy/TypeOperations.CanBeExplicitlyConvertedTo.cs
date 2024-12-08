using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public static partial class TypeOperations
{
    public static bool CanBeExplicitlyConvertedTo(this IMaybeType fromType, IMaybeType toType, bool safeOnly)
    {
        return (fromType, toType) switch
        {
            // Safe conversions
            (CapabilityType { TypeConstructor: BoolTypeConstructor }, CapabilityType { TypeConstructor: IntegerTypeConstructor }) => true,
            (CapabilityType { TypeConstructor: BoolLiteralTypeConstructor }, CapabilityType { TypeConstructor: IntegerTypeConstructor }) => true,
            (CapabilityType { TypeConstructor: IntegerTypeConstructor { IsSigned: false } }, CapabilityType { TypeConstructor: BigIntegerTypeConstructor })
                => true,
            (CapabilityType { TypeConstructor: IntegerTypeConstructor }, CapabilityType { TypeConstructor: BigIntegerTypeConstructor { IsSigned: true } })
                => true,
            (CapabilityType { TypeConstructor: FixedSizeIntegerTypeConstructor from }, CapabilityType { TypeConstructor: FixedSizeIntegerTypeConstructor to })
                when from.Bits < to.Bits
                     || (from.Bits == to.Bits && from.IsSigned == to.IsSigned)
                => true,

            // TODO conversions for constants
            // Unsafe conversions
            (CapabilityType { TypeConstructor: IntegerTypeConstructor }, CapabilityType { TypeConstructor: IntegerTypeConstructor }) => !safeOnly,

            // Of course, anything that can be assigned is also fine
            _ => toType.IsAssignableFrom(fromType),
        };
    }
}
