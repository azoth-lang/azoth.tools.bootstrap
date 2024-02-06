using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

public sealed class ObjectTypeConstraint : Pseudotype
{
    public BareObjectType BareType { get; }

    public override bool IsFullyKnown => BareType.IsFullyKnown;

    public ReferenceCapabilityConstraint Capability { get; }

    public ObjectTypeConstraint(ReferenceCapabilityConstraint capability, BareObjectType bareType)
    {
        Capability = capability;
        BareType = bareType;
    }

    public override DataType ToUpperBound()
        => ObjectType.Create(ReferenceCapability.ReadOnly, BareType);

    public override string ToILString() => $"{Capability} {BareType.ToILString()}";

    public override string ToSourceCodeString() => $"{Capability} {BareType.ToSourceCodeString()}";
}
