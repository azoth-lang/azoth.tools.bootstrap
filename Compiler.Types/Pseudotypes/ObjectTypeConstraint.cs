using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

public sealed class ObjectTypeConstraint : Pseudotype
{
    public BareType BareType { get; }

    public override bool IsFullyKnown => BareType.IsFullyKnown;

    public ReferenceCapabilityConstraint Capability { get; }

    public ObjectTypeConstraint(ReferenceCapabilityConstraint capability, BareType bareType)
    {
        Capability = capability;
        BareType = bareType;
    }

    public override DataType ToUpperBound()
        => BareType.With(ReferenceCapability.Read);

    public override string ToILString() => $"{Capability} {BareType.ToILString()}";

    public override string ToSourceCodeString() => $"{Capability} {BareType.ToSourceCodeString()}";
}
