using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// e.g. `self |> mut Foo` Applies to all non-void types
public sealed class SelfViewpointType : NonVoidType
{
    public static IMaybeType Create(CapabilitySet capability, IMaybeType referent)
        => referent switch
        {
            NonVoidType t => new SelfViewpointType(capability, t),
            VoidType _ => Type.Void,
            UnknownType _ => Type.Unknown,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public CapabilitySet Capability { get; }

    public NonVoidType Referent { get; }

    public override NonVoidPlainType PlainType => Referent.PlainType;

    public override BareTypeReplacements TypeReplacements => Referent.TypeReplacements;

    public override bool HasIndependentTypeArguments => Referent.HasIndependentTypeArguments;

    public SelfViewpointType(CapabilitySet capability, NonVoidType referent)
    {
        Capability = capability;
        Referent = referent;
    }

    #region Equality
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SelfViewpointType otherType
               && Capability.Equals(otherType.Capability)
               && Referent.Equals(otherType.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(Capability, Referent);
    #endregion

    public override string ToSourceCodeString() => $"{Capability.ToSourceCodeString()} self |> {Referent}";

    public override string ToILString() => $"{Capability.ToILString()} |> {Referent}";
}
