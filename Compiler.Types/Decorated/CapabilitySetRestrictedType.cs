using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

/// <summary>
/// A generic type upcast to a capability in the given capability set (e.g. `shareable T`)
/// </summary>
public sealed class CapabilitySetRestrictedType : NonVoidType
{
    [return: NotNullIfNotNull(nameof(referent))]
    public static NonVoidType? Create(CapabilitySetWithIdentity capabilitySet, GenericParameterType? referent)
        => referent is null ? null : new CapabilitySetRestrictedType(capabilitySet, referent);

    public override NonVoidType? BaseType => null;

    public CapabilitySetWithIdentity CapabilitySet { [DebuggerStepThrough] get; }

    public GenericParameterType Referent { [DebuggerStepThrough] get; }

    public override GenericParameterPlainType PlainType => Referent.PlainType;

    public override bool HasIndependentTypeArguments => false;

    internal override GenericParameterTypeReplacements BareTypeReplacements => Referent.BareTypeReplacements;

    public CapabilitySetRestrictedType(CapabilitySetWithIdentity capabilitySet, GenericParameterType referent)
    {
        Requires.That(!referent.ImplicitConstraint.IsSubtypeOf(capabilitySet), nameof(capabilitySet),
            $"Cannot apply capability set '{capabilitySet}' to type with implicit constraint of '{referent.ImplicitConstraint}' because it is redundant.");
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
        => $"{CapabilitySet.ToSourceCodeString()} {Referent.PlainType}";

    public override string ToILString() => $"{CapabilitySet.ToILString()} {Referent.PlainType}";
}
