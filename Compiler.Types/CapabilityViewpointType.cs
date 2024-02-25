using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class CapabilityViewpointType : ViewpointType
{
    public static DataType Create(Capability capability, GenericParameterType referent)
    {
        if (capability == Capabilities.Capability.Mutable || capability == Capabilities.Capability.InitMutable)
            return referent;
        return new CapabilityViewpointType(capability, referent);
    }

    public override Capability Capability { get; }

    public override GenericParameterType Referent { get; }

    private CapabilityViewpointType(Capability capability, GenericParameterType referent)
    {
        Capability = capability;
        Referent = referent;
    }

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
        => $"{Capability.ToSourceCodeString()}|>{Referent.ToSourceCodeString()}";

    public override string ToILString() => $"{Capability.ToILString()}|>{Referent.ToILString()}";
}
