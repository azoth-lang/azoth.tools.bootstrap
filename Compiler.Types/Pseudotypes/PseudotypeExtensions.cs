using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

public static class PseudotypeExtensions
{
    public static bool IsAssignableFrom(this IMaybePseudotype target, IMaybePseudotype source)
    {
        return (target, source) switch
        {
            (IMaybeExpressionType targetType, IMaybeExpressionType sourceType) => DataTypeExtensions.IsAssignableFrom(targetType, sourceType),
            (CapabilityTypeConstraint targetType, CapabilityType sourceType) => targetType.IsAssignableFrom(sourceType),
            (CapabilityTypeConstraint targetType, CapabilityTypeConstraint sourceType) => targetType.IsAssignableFrom(sourceType),
            _ => false,
        };
    }

    public static bool IsAssignableFrom(this CapabilityTypeConstraint target, IMaybePseudotype source)
    {
        return source switch
        {
            CapabilityType objectType => target.IsAssignableFrom(objectType),
            CapabilityTypeConstraint objectTypeConstraint => target.IsAssignableFrom(objectTypeConstraint),
            IMaybeExpressionType _ => false,
            _ => throw ExhaustiveMatch.Failed(source),
        };
    }

    public static bool IsAssignableFrom(this CapabilityTypeConstraint target, CapabilityType source)
        => target.BareType.IsAssignableFrom(target.Capability.AnyCapabilityAllowsWrite, source.BareType)
           && target.Capability.IsAssignableFrom(source.Capability);

    public static bool IsAssignableFrom(this CapabilityTypeConstraint target, CapabilityTypeConstraint source)
        => target.BareType.IsAssignableFrom(target.Capability.AnyCapabilityAllowsWrite, source.BareType)
           && target.Capability.IsAssignableFrom(source.Capability);

    public static bool IsAssignableFrom(this CapabilityType target, IMaybePseudotype source)
    {
        return source switch
        {
            IMaybeExpressionType sourceType => target.IsAssignableFrom(sourceType),
            CapabilityTypeConstraint capabilityTypeConstraint
                => target.BareType.IsAssignableFrom(target.AllowsWrite, capabilityTypeConstraint.BareType),
            _ => throw ExhaustiveMatch.Failed(source)
        };
    }
}
