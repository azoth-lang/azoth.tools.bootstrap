using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

public sealed class ObjectTypeConstraint : Pseudotype
{
    public BareType BareType { get; }

    public override bool IsFullyKnown => BareType.IsFullyKnown;

    public CapabilitySet Capability { get; }

    public ObjectTypeConstraint(CapabilitySet capability, BareType bareType)
    {
        Capability = capability;
        BareType = bareType;
    }

    public override DataType ToUpperBound()
        => BareType.With(Capabilities.Capability.Read);

    public override string ToILString() => $"{Capability} {BareType.ToILString()}";

    public override string ToSourceCodeString() => $"{Capability} {BareType.ToSourceCodeString()}";
}
