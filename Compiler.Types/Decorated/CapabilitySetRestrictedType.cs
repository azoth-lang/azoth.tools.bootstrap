using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

/// <summary>
/// A generic type upcast to a capability in the given capability set (e.g. `shareable T`)
/// </summary>
public sealed class CapabilitySetRestrictedType : NonVoidType
{
    public static IMaybeType Create(CapabilitySet capabilitySet, IMaybeType referent)
      => referent switch
      {
          GenericParameterType t => Create(capabilitySet, t),
          // In an error case, don't actually make a capability set restricted type.
          _ => referent.AccessedVia(capabilitySet),
      };

    public static NonVoidType Create(CapabilitySet capabilitySet, GenericParameterType referent)
        => new CapabilitySetRestrictedType(capabilitySet, referent);

    public CapabilitySet CapabilitySet { [DebuggerStepThrough] get; }

    public GenericParameterType Referent { [DebuggerStepThrough] get; }

    public override GenericParameterPlainType PlainType => Referent.PlainType;

    public override bool HasIndependentTypeArguments => false;

    internal override GenericParameterTypeReplacements BareTypeReplacements => Referent.BareTypeReplacements;

    private CapabilitySetRestrictedType(CapabilitySet capabilitySet, GenericParameterType referent)
    {
        Requires.That(referent is not { Parameter.Independence: TypeParameterIndependence.Independent }, nameof(referent),
            "Must not be a fully independent generic parameter.");
        if (capabilitySet != CapabilitySet.Shareable)
            Requires.That(referent is not { Parameter.Independence: TypeParameterIndependence.ShareableIndependent }, nameof(referent),
                "Must not be a fully shareable independent generic parameter.");
        CapabilitySet = capabilitySet;
        Referent = referent;
    }

    #region Equals
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilitySetRestrictedType otherType
               && CapabilitySet == otherType.CapabilitySet
               && Referent.Equals(otherType.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(CapabilitySet, Referent);
    #endregion

    public override string ToSourceCodeString()
        => $"{CapabilitySet.ToSourceCodeString()} {Referent.ToSourceCodeString()}";

    public override string ToILString() => $"{CapabilitySet.ToILString()} {Referent.ToILString()}";
}
