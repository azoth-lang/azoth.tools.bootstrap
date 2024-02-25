using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

public static class PseudotypeExtensions
{
    public static bool IsAssignableFrom(this Pseudotype target, Pseudotype source)
    {
        return (target, source) switch
        {
            (DataType targetType, DataType sourceType) => DataTypeExtensions.IsAssignableFrom(targetType, sourceType),
            (ObjectTypeConstraint targetType, ReferenceType sourceType) => targetType.IsAssignableFrom(sourceType),
            (ObjectTypeConstraint targetType, ObjectTypeConstraint sourceType) => targetType.IsAssignableFrom(sourceType),
            _ => false,
        };
    }

    public static bool IsAssignableFrom(this ObjectTypeConstraint target, Pseudotype source)
    {
        return source switch
        {
            ReferenceType objectType => target.IsAssignableFrom(objectType),
            ObjectTypeConstraint objectTypeConstraint => target.IsAssignableFrom(objectTypeConstraint),
            DataType _ => false,
            _ => throw ExhaustiveMatch.Failed(source),
        };
    }

    public static bool IsAssignableFrom(this ObjectTypeConstraint target, ReferenceType source)
        => target.BareType.IsAssignableFrom(target.Capability.AnyCapabilityAllowsWrite, source.BareType)
           && target.Capability.IsAssignableFrom(source.Capability);

    public static bool IsAssignableFrom(this ObjectTypeConstraint target, ObjectTypeConstraint source)
        => target.BareType.IsAssignableFrom(target.Capability.AnyCapabilityAllowsWrite, source.BareType)
           && target.Capability.IsAssignableFrom(source.Capability);

    public static bool IsAssignableFrom(this ReferenceType target, Pseudotype source)
    {
        return source switch
        {
            DataType sourceType => target.IsAssignableFrom(sourceType),
            ObjectTypeConstraint constrainedObjectType
                => target.BareType.IsAssignableFrom(target.AllowsWrite, constrainedObjectType.BareType),
            _ => throw ExhaustiveMatch.Failed(source)
        };
    }
}
