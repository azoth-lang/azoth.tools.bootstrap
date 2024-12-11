using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// e.g. `self |> mut Foo` Applies to all non-void types
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class SelfViewpointType : INonVoidType
{
    public static IMaybeType Create(CapabilitySet capability, IMaybeType referent)
        => referent switch
        {
            INonVoidType t => new SelfViewpointType(capability, t),
            VoidType _ => IType.Void,
            UnknownType _ => IType.Unknown,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public CapabilitySet Capability { get; }

    public INonVoidType Referent { get; }

    public INonVoidPlainType PlainType => Referent.PlainType;
    IMaybePlainType IMaybeType.PlainType => PlainType;

    public TypeReplacements TypeReplacements => Referent.TypeReplacements;

    public bool HasIndependentTypeArguments => Referent.HasIndependentTypeArguments;

    public SelfViewpointType(CapabilitySet capability, INonVoidType referent)
    {
        Capability = capability;
        Referent = referent;
    }

    #region Equality
    public bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SelfViewpointType otherType
               && Capability.Equals(otherType.Capability)
               && Referent.Equals(otherType.Referent);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is SelfViewpointType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Capability, Referent);
    #endregion

    public override string ToString() => ToILString();

    public string ToSourceCodeString() => $"{Capability.ToSourceCodeString()} self |> {Referent}";

    public string ToILString() => $"{Capability.ToILString()} |> {Referent}";
}
