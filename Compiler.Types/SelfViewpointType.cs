using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class SelfViewpointType : ViewpointType
{
    public static IMaybeExpressionType Create(CapabilitySet capability, IMaybeExpressionType referent)
        => referent switch
        {
            Type t => new SelfViewpointType(capability, t),
            UnknownType _ => IType.Unknown,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public static Type Create(CapabilitySet capability, Type referent)
        => new SelfViewpointType(capability, referent);

    public override CapabilitySet Capability { get; }

    public override Type Referent { get; }

    // TODO do not allow self viewpoint of constant value types
    public SelfViewpointType(CapabilitySet capability, Type referent)
    {
        Capability = capability;
        Referent = referent;
    }

    #region Equals
    public override bool Equals(IMaybeExpressionType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SelfViewpointType otherType
               && Capability == otherType.Capability
               && Referent.Equals(otherType.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(Capability, Referent);
    #endregion

    public override string ToSourceCodeString()
        => $"{Capability.ToSourceCodeString()} self|>{Referent.ToSourceCodeString()}";

    public override string ToILString()
        => $"{Capability.ToILString()} self|>{Referent.ToILString()}";
}
