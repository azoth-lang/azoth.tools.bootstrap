using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// e.g. `readable Self`
// Applies to SelfPlainType
// Could be CapabilitySelfType and operate on an ICapabilityConstraint but that
// would introduce two ways of having types with capabilities on them.
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
// TODO maybe this should merge with the SelfViewpointType the way CapabilityType can sometimes be a viewpoint
public sealed class CapabilitySetSelfType : INonVoidType
{
    public CapabilitySet Capability { get; }
    public SelfPlainType PlainType { get; }

    NonVoidPlainType INonVoidType.PlainType => PlainType;
    IMaybePlainType IMaybeType.PlainType => PlainType;

    public TypeReplacements TypeReplacements => TypeReplacements.None;

    public bool HasIndependentTypeArguments => false;

    public CapabilitySetSelfType(CapabilitySet capability, SelfPlainType plainType)
    {
        Capability = capability;
        PlainType = plainType;
    }

    #region Equality
    public bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilitySetSelfType otherType
               && Capability.Equals(otherType.Capability)
               && PlainType.Equals(otherType.PlainType);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is CapabilitySetSelfType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Capability, PlainType);
    #endregion

    public override string ToString() => ToILString();

    public string ToSourceCodeString() => $"{Capability.ToSourceCodeString()} {PlainType}";

    public string ToILString() => $"{Capability.ToILString()} {PlainType}";
}
