using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

/// <summary>
/// A generic type as accessed from `self` when it has a capability set self type (e.g. `self |> T`).
/// The target type can be any non-void type.
/// </summary>
// TODO maybe this shouldn't reference the capability set. That is implied by the context?
public sealed class SelfViewpointType : NonVoidType
{
    public static IMaybeType Create(CapabilitySet capability, IMaybeType referent)
        => referent switch
        {
            NonVoidType t => new SelfViewpointType(capability, t),
            VoidType _ => Void,
            UnknownType _ => Unknown,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public static Type Create(CapabilitySet capability, Type referent)
        => referent switch
        {
            NonVoidType t => new SelfViewpointType(capability, t),
            VoidType _ => Void,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public static SelfViewpointType? Create(CapabilitySet capability, NonVoidType? referent)
        => referent is not null ? new SelfViewpointType(capability, referent) : null;

    public override NonVoidType? BaseType => Create(CapabilitySet, Referent.BaseType);

    public CapabilitySet CapabilitySet { [DebuggerStepThrough] get; }

    public NonVoidType Referent { [DebuggerStepThrough] get; }

    public override NonVoidPlainType PlainType => Referent.PlainType;

    internal override GenericParameterTypeReplacements BareTypeReplacements => Referent.BareTypeReplacements;

    public override bool HasIndependentTypeArguments => Referent.HasIndependentTypeArguments;

    // TODO switch to requiring a plain type
    public SelfViewpointType(CapabilitySet capabilitySet, NonVoidType referent)
    {
        CapabilitySet = capabilitySet;
        Referent = referent;
    }

    #region Equality
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SelfViewpointType otherType
               && CapabilitySet.Equals(otherType.CapabilitySet)
               && Referent.Equals(otherType.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(CapabilitySet, Referent);
    #endregion

    public override string ToSourceCodeString() => $"{CapabilitySet.ToSourceCodeString()} self |> {Referent}";

    public override string ToILString() => $"{CapabilitySet.ToILString()} |> {Referent}";
}
