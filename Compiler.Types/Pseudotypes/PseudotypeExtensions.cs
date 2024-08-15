using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

public static class PseudotypeExtensions
{
    public static bool IsAssignableFrom(this Pseudotype target, Pseudotype source)
    {
        return (target, source) switch
        {
            (DataType targetType, DataType sourceType) => DataTypeExtensions.IsAssignableFrom(targetType, sourceType),
            (CapabilityTypeConstraint targetType, CapabilityType sourceType) => targetType.IsAssignableFrom(sourceType),
            (CapabilityTypeConstraint targetType, CapabilityTypeConstraint sourceType) => targetType.IsAssignableFrom(sourceType),
            _ => false,
        };
    }

    public static bool IsAssignableFrom(this CapabilityTypeConstraint target, Pseudotype source)
    {
        return source switch
        {
            CapabilityType objectType => target.IsAssignableFrom(objectType),
            CapabilityTypeConstraint objectTypeConstraint => target.IsAssignableFrom(objectTypeConstraint),
            DataType _ => false,
            _ => throw ExhaustiveMatch.Failed(source),
        };
    }

    public static bool IsAssignableFrom(this CapabilityTypeConstraint target, CapabilityType source)
        => target.BareType.IsAssignableFrom(target.Capability.AnyCapabilityAllowsWrite, source.BareType)
           && target.Capability.IsAssignableFrom(source.Capability);

    public static bool IsAssignableFrom(this CapabilityTypeConstraint target, CapabilityTypeConstraint source)
        => target.BareType.IsAssignableFrom(target.Capability.AnyCapabilityAllowsWrite, source.BareType)
           && target.Capability.IsAssignableFrom(source.Capability);

    public static bool IsAssignableFrom(this CapabilityType target, Pseudotype source)
    {
        return source switch
        {
            DataType sourceType => target.IsAssignableFrom(sourceType),
            CapabilityTypeConstraint capabilityTypeConstraint
                => target.BareType.IsAssignableFrom(target.AllowsWrite, capabilityTypeConstraint.BareType),
            _ => throw ExhaustiveMatch.Failed(source)
        };
    }
}
