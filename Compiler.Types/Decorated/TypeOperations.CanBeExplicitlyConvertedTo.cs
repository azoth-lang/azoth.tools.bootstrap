using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    /// <summary>
    /// Whether a simple type supports an explicit conversion to another simple type.
    /// </summary>
    // TODO rename to reflect that this is only for simple types
    // TODO maybe these explicit conversions should be handled by adding conversion members to the types
    public static bool CanBeExplicitlyConvertedTo(this IMaybeType fromType, IMaybeType toType, bool safeOnly)
    {
        return CanBeExplicitlyConverted(fromType, toType, safeOnly, false);

        static bool CanBeExplicitlyConverted(IMaybeType fromType, IMaybeType toType, bool safeOnly, bool recursive)
            => (fromType, toType) switch
            {
                // TODO organize conversion code and remove any duplication between types and plain types

                // Unsafe conversions
                (CapabilityType { TypeConstructor: IntegerTypeConstructor }, CapabilityType { TypeConstructor: IntegerTypeConstructor }) when !safeOnly
                    => true,

                // Safe conversions
                // TODO add floating point conversions
                (CapabilityType { TypeConstructor: BoolTypeConstructor }, CapabilityType { TypeConstructor: IntegerTypeConstructor }) => true,
                (CapabilityType { TypeConstructor: BoolLiteralTypeConstructor }, CapabilityType { TypeConstructor: IntegerTypeConstructor }) => true,
                // All implicit conversions are valid explicit conversions
                (CapabilityType { TypeConstructor: SimpleOrLiteralTypeConstructor } from, CapabilityType { TypeConstructor: SimpleOrLiteralTypeConstructor } to)
                    => from.PlainType.IsImplicitlyConvertibleTo(to.PlainType),

                // Lifted conversions
                (OptionalType from, OptionalType to)
                    => CanBeExplicitlyConverted(from.Referent, to.Referent, safeOnly, recursive: true),

                // An optional conversion is fine as long as the underlying types are convertible
                (Type from, OptionalType toOptionalType)
                    // TODO this should be IsAssignableTo or something
                    => (recursive || from.Semantics != TypeSemantics.Reference)
                       && CanBeExplicitlyConverted(fromType, toOptionalType.Referent, safeOnly, recursive: true),

                // TODO this is a little odd, if it is a subtype, it doesn't need converted
                // Of course, anything that can be assigned is also fine
                _ when recursive => fromType.IsSubtypeOf(toType),

                _ => false,
            };
    }
}
