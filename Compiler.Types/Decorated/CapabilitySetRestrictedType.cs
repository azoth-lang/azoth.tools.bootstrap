using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

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

    public override NonVoidType? BaseType => null;

    public CapabilitySet CapabilitySet { [DebuggerStepThrough] get; }

    public GenericParameterType Referent { [DebuggerStepThrough] get; }

    public override GenericParameterPlainType PlainType => Referent.PlainType;

    public override bool HasIndependentTypeArguments => false;

    internal override GenericParameterTypeReplacements BareTypeReplacements => Referent.BareTypeReplacements;

    private CapabilitySetRestrictedType(CapabilitySet capabilitySet, GenericParameterType referent)
    {
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
