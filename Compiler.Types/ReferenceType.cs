using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(
    typeof(ObjectType),
    typeof(AnyType))]
public abstract class ReferenceType : NonEmptyType
{
    public ReferenceCapability Capability { get; }
    public bool IsReadOnlyReference => Capability == ReferenceCapability.ReadOnly;
    public bool IsConstReference => Capability == ReferenceCapability.Constant;
    public bool IsIsolatedReference => Capability == ReferenceCapability.Isolated;
    public bool IsIdentityReference => Capability == ReferenceCapability.Identity;

    public bool AllowsWrite => Capability.AllowsWrite;

    /// <summary>
    /// Does this reference allow it to be recovered to isolated if reference sharing permits.
    /// </summary>
    public bool AllowsRecoverIsolation => Capability.AllowsRecoverIsolation;

    /// <summary>
    /// Does this capability allow a reference with it to be frozen to const if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsFreeze => Capability.AllowsFreeze;

    public virtual DeclaredReferenceType DeclaredType { get; }

    public virtual TypeName Name => DeclaredType.Name;

    public override TypeSemantics Semantics => TypeSemantics.Reference;

    private protected ReferenceType(ReferenceCapability capability, DeclaredReferenceType declaredType)
    {
        Capability = capability;
        DeclaredType = declaredType;
    }

    public ReferenceType ToMutable()
    {
        if (!Capability.AllowsWrite)
            throw new InvalidOperationException($"Can't convert '{Capability.ToILString()}' to mutable because it does not allow write.");
        return To(ReferenceCapability.Mutable);
    }

    public override ReferenceType WithoutWrite() => To(Capability.WithoutWrite());

    /// <summary>
    /// Return the same type except with the given reference capability
    /// </summary>
    public abstract ReferenceType To(ReferenceCapability referenceCapability);

    public bool BareTypeEquals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ObjectType otherType
            && DeclaredType == otherType.DeclaredType;
    }
}
