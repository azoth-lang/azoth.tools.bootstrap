using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class SelfViewpointType : ViewpointType
{
    public override ReferenceCapabilityConstraint Capability { get; }

    public override DataType Referent { get; }

    public SelfViewpointType(ReferenceCapabilityConstraint capability, DataType referent)
    {
        Capability = capability;
        Referent = referent;
    }

    #region Equals
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SelfViewpointType otherType
               && Capability == otherType.Capability
               && Referent == otherType.Referent;
    }

    public override int GetHashCode() => HashCode.Combine(Capability, Referent);
    #endregion

    public override string ToSourceCodeString()
        => $"{Capability} self|>{Referent.ToSourceCodeString()}";

    public override string ToILString()
        => $"{Capability} self|>{Referent.ToILString()}";
}
