using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(
    typeof(ObjectType),
    typeof(AnyType))]
public abstract class ReferenceType : NonEmptyType
{
    public ReferenceCapability Capability { get; }
    public bool IsReadOnlyReference => Capability.IsReadOnly;
    public bool IsConstReference => Capability.IsConstant;
    public bool IsIsolatedReference => Capability.IsIsolated;
    public bool IsIdentityReference => Capability == ReferenceCapability.Identity;
    public bool IsInitReference => Capability.IsInit;
    public override bool AllowsWrite => Capability.AllowsWrite;

    public override bool AllowsWriteAliases => Capability.AllowsWriteAliases;

    /// <summary>
    /// Does this reference allow it to be recovered to isolated if reference sharing permits.
    /// </summary>
    public bool AllowsRecoverIsolation => Capability.AllowsRecoverIsolation;

    /// <summary>
    /// Does this capability allow a reference with it to be moved if reference sharing permits.
    /// </summary>
    public bool AllowsMove => Capability.AllowsMove;

    /// <summary>
    /// Does this capability allow a reference with it to be frozen to const if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsFreeze => Capability.AllowsFreeze;

    public virtual DeclaredReferenceType DeclaredType { get; }

    public SimpleName? ContainingPackage => DeclaredType.ContainingPackage;
    public NamespaceName ContainingNamespace => DeclaredType.ContainingNamespace;

    public virtual Name Name => DeclaredType.Name;

    public override TypeSemantics Semantics => TypeSemantics.Reference;

    private protected ReferenceType(ReferenceCapability capability, DeclaredReferenceType declaredType)
    {
        Capability = capability;
        DeclaredType = declaredType;
    }

    public override ReferenceType WithoutWrite() => With(Capability.WithoutWrite());

    /// <summary>
    /// Return the same type except with the given reference capability
    /// </summary>
    public abstract ReferenceType With(ReferenceCapability referenceCapability);
}
