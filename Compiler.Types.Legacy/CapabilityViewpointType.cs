using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

// TODO should `iso|>T` be disallowed?
public sealed class CapabilityViewpointType : ViewpointType
{
    public static IMaybeType Create(Capability capability, IMaybeType referent)
        => referent switch
        {
            GenericParameterType t => Create(capability, t),
            // In an error case, don't actually make a capability type.
            // TODO doesn't this need to combine the capabilities? (th
            _ => referent,
        };

    public static INonVoidType Create(Capability capability, GenericParameterType referent)
    {
        if (capability == Capability.Mutable || capability == Capability.InitMutable)
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

    public override Decorated.CapabilityType ToDecoratedType()
        => new Decorated.CapabilityType(Capability, ToPlainType(), []);

    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilityViewpointType otherType
               && Capability == otherType.Capability
               && Referent.Equals(otherType.Referent);
    }

    public override int GetHashCode()
        => HashCode.Combine(Capability, Referent);
    #endregion

    public override string ToSourceCodeString()
        => $"{Capability.ToSourceCodeString()}|>{Referent.ToSourceCodeString()}";

    public override string ToILString() => $"{Capability.ToILString()}|>{Referent.ToILString()}";
    public override GenericParameterPlainType ToPlainType() => Referent.ToPlainType();
}
