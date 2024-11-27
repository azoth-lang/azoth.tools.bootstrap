using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;

public sealed class CapabilityTypeConstraint : IPseudotype
{
    public BareNonVariableType BareType { get; }

    public CapabilitySet Capability { get; }

    public CapabilityTypeConstraint(CapabilitySet capability, BareNonVariableType bareType)
    {
        Capability = capability;
        BareType = bareType;
    }

    public CapabilityType ToUpperBound()
        => BareType.With(Capability.UpperBound);
    IExpressionType IPseudotype.ToUpperBound() => ToUpperBound();

    public IMaybeExpressionAntetype ToAntetype() => BareType.ToAntetype();

    #region Equality
    public bool Equals(IMaybePseudotype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilityTypeConstraint capabilityTypeConstraint
               && Capability.Equals(capabilityTypeConstraint.Capability)
               && BareType.Equals(capabilityTypeConstraint.BareType);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is IMaybePseudotype other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Capability, BareType);
    #endregion

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => $"{Capability} {BareType.ToILString()}";

    public string ToSourceCodeString() => $"{Capability} {BareType.ToSourceCodeString()}";
}
