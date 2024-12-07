using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public static partial class TypeOperations
{
    public static bool CanBeExplicitlyConvertedTo(this IMaybeType fromType, IMaybeType toType, bool safeOnly)
    {
        return (fromType, toType) switch
        {
            // Safe conversions
            (CapabilityType { DeclaredType: BoolType }, CapabilityType { DeclaredType: IntegerType }) => true,
            (BoolConstValueType, CapabilityType { DeclaredType: IntegerType }) => true,
            (CapabilityType { DeclaredType: IntegerType { IsSigned: false } }, CapabilityType { DeclaredType: BigIntegerType })
                => true,
            (CapabilityType { DeclaredType: IntegerType }, CapabilityType { DeclaredType: BigIntegerType { IsSigned: true } })
                => true,
            (CapabilityType { DeclaredType: FixedSizeIntegerType from }, CapabilityType { DeclaredType: FixedSizeIntegerType to })
                when from.Bits < to.Bits
                     || (from.Bits == to.Bits && from.IsSigned == to.IsSigned)
                => true,

            // TODO conversions for constants
            // Unsafe conversions
            (CapabilityType { DeclaredType: IntegerType }, CapabilityType { DeclaredType: IntegerType }) => !safeOnly,

            // Of course, anything that can be assigned is also fine
            _ => toType.IsAssignableFrom(fromType),
        };
    }
}
