using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

public sealed class CapabilityTypeConstraint : Pseudotype
{
    public BareType BareType { get; }

    public override bool IsFullyKnown => BareType.IsFullyKnown;

    public CapabilitySet Capability { get; }

    public CapabilityTypeConstraint(CapabilitySet capability, BareType bareType)
    {
        Capability = capability;
        BareType = bareType;
    }

    public override DataType ToUpperBound()
        => BareType.With(Capability.UpperBound);

    public override string ToILString() => $"{Capability} {BareType.ToILString()}";

    public override string ToSourceCodeString() => $"{Capability} {BareType.ToSourceCodeString()}";
}
