using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public sealed class SelfViewpointType : ViewpointType
{
    public static IMaybeType Create(CapabilitySet capability, IMaybeType referent)
        => referent switch
        {
            INonVoidType t => new SelfViewpointType(capability, t),
            VoidType _ => IType.Void,
            UnknownType _ => IType.Unknown,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public static IType Create(CapabilitySet capability, IType referent)
        => referent switch
        {
            INonVoidType t => new SelfViewpointType(capability, t),
            VoidType _ => IType.Void,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public static IMaybeNonVoidType Create(CapabilitySet capability, IMaybeNonVoidType referent)
        => referent switch
        {
            INonVoidType t => new SelfViewpointType(capability, t),
            UnknownType _ => IType.Unknown,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public static IType Create(CapabilitySet capability, INonVoidType referent)
        => new SelfViewpointType(capability, referent);

    public override CapabilitySet Capability { get; }

    public override INonVoidType Referent { get; }

    private SelfViewpointType(CapabilitySet capability, INonVoidType referent)
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
        => $"self|>{Referent.ToSourceCodeString()}";

    public override string ToILString()
        => $"{Capability.ToILString()} self|>{Referent.ToILString()}";
}
