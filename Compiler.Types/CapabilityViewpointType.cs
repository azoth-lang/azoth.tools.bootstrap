using System;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class CapabilityViewpointType : ViewpointType
{
    public static DataType Create(ReferenceCapability capability, GenericParameterType referent)
    {
        if (capability == ReferenceCapability.Mutable || capability == ReferenceCapability.InitMutable)
            return referent;
        return new CapabilityViewpointType(capability, referent);
    }

    private CapabilityViewpointType(ReferenceCapability capability, GenericParameterType referent)
        : base(referent)
    {
        Capability = capability;
    }

    public ReferenceCapability Capability { get; }

    #region Equals
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilityViewpointType otherType
               && Capability == otherType.Capability
               && Referent == otherType.Referent;
    }

    public override int GetHashCode()
        => HashCode.Combine(Capability, Referent);
    #endregion


    public override string ToSourceCodeString()
        => $"{Capability.ToSourceString()}|>{Referent.ToSourceCodeString()}";

    public override string ToILString() => $"{Capability.ToILString()}|>{Referent.ToILString()}";
}
