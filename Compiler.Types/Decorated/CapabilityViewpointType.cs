using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// TODO should `iso|>T` be disallowed?
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class CapabilityViewpointType : INonVoidType
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
        // TODO is this really a result of the implicit `aliasable` constraint on most type parameters?
        if (capability == Capability.Mutable || capability == Capability.InitMutable) return referent;
        return new CapabilityViewpointType(capability, referent);
    }

    public Capability Capability { get; }

    public GenericParameterType Referent { get; }

    public GenericParameterPlainType PlainType => Referent.PlainType;
    NonVoidPlainType INonVoidType.PlainType => PlainType;
    IMaybePlainType IMaybeType.PlainType => PlainType;

    public bool HasIndependentTypeArguments => false;

    public TypeReplacements TypeReplacements => TypeReplacements.None;

    private CapabilityViewpointType(Capability capability, GenericParameterType referent)
    {
        Requires.That(referent is not { Parameter.HasIndependence: true }, nameof(referent),
            "Must not be an independent generic parameter.");
        Capability = capability;
        Referent = referent;
    }

    #region Equals
    public bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilityViewpointType otherType
               && Capability == otherType.Capability
               && Referent.Equals(otherType.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(Capability, Referent);
    #endregion

    public override string ToString() => ToILString();

    public string ToSourceCodeString()
        => $"{Capability.ToSourceCodeString()} |> {Referent.ToSourceCodeString()}";

    public string ToILString() => $"{Capability.ToILString()} |> {Referent.ToILString()}";
}
