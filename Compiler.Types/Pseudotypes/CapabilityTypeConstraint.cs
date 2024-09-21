using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

public sealed class CapabilityTypeConstraint : Pseudotype, IPseudotype
{
    public BareType BareType { get; }

    public override bool IsFullyKnown => BareType.IsFullyKnown;

    public CapabilitySet Capability { get; }

    public CapabilityTypeConstraint(CapabilitySet capability, BareType bareType)
    {
        Capability = capability;
        BareType = bareType;
    }

    public override CapabilityType ToUpperBound()
        => BareType.With(Capability.UpperBound);

    public override IMaybeExpressionAntetype ToAntetype() => BareType.ToAntetype();

    #region Equality
    public override bool Equals(IMaybePseudotype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilityTypeConstraint capabilityTypeConstraint
               && Capability.Equals(capabilityTypeConstraint.Capability)
               && BareType.Equals(capabilityTypeConstraint.BareType);
    }

    public override int GetHashCode() => HashCode.Combine(Capability, BareType);
    #endregion

    public override string ToILString() => $"{Capability} {BareType.ToILString()}";

    public override string ToSourceCodeString() => $"{Capability} {BareType.ToSourceCodeString()}";
}
