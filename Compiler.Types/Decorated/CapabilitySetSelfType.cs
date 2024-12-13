using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

/// <summary>
/// A self type with a capability set (e.g. `readable Self`).
/// </summary>
/// <remarks><para>This is used as the type of the self parameter of methods that use capability
/// sets on the self parameter.</para>
///
/// <para>This is distinct from <see cref="SelfViewpointType"/> even though that combines a
/// capability set with a type because that combines with a full type whereas this combines with a
/// plain type.</para></remarks>
// TODO maybe this should be based on a bare type for consistency with CapabilityType
// TODO this needs type argument capabilities for the containing type
public sealed class CapabilitySetSelfType : NonVoidType
{
    public CapabilitySet Capability { get; }

    public override SelfPlainType PlainType { get; }

    public override TypeReplacements TypeReplacements => TypeReplacements.None;

    public override bool HasIndependentTypeArguments => false;

    public CapabilitySetSelfType(CapabilitySet capability, SelfPlainType plainType)
    {
        Capability = capability;
        PlainType = plainType;
    }

    #region Equality
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilitySetSelfType otherType
               && Capability.Equals(otherType.Capability)
               && PlainType.Equals(otherType.PlainType);
    }

    public override int GetHashCode() => HashCode.Combine(Capability, PlainType);
    #endregion

    public override string ToSourceCodeString() => $"{Capability.ToSourceCodeString()} {PlainType}";

    public override string ToILString() => $"{Capability.ToILString()} {PlainType}";
}
