using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
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

    public virtual BareReferenceType BareType { get; }

    public virtual DeclaredReferenceType DeclaredType => BareType.DeclaredType;

    public FixedList<DataType> TypeArguments => BareType.TypeArguments;

    public FixedSet<BareReferenceType> Supertypes => BareType.Supertypes;

    public override bool IsFullyKnown => BareType.IsFullyKnown;

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// const.
    /// </summary>
    public bool IsConstType => DeclaredType.IsConstType;

    public SimpleName? ContainingPackage => DeclaredType.ContainingPackage;
    public NamespaceName ContainingNamespace => DeclaredType.ContainingNamespace;

    public virtual Name Name => DeclaredType.Name;

    public override TypeSemantics Semantics => TypeSemantics.Reference;

    private protected ReferenceType(ReferenceCapability capability, BareReferenceType bareType)
    {
        Capability = capability;
        BareType = bareType;
    }

    public override ReferenceType WithoutWrite() => With(Capability.WithoutWrite());

    /// <summary>
    /// Return the same type except with the given reference capability
    /// </summary>
    public abstract ReferenceType With(ReferenceCapability referenceCapability);

    public sealed override string ToSourceCodeString()
    {
        if (Capability != ReferenceCapability.ReadOnly)
            return $"{Capability.ToSourceString()} {BareType.ToSourceCodeString()}";

        return BareType.ToSourceCodeString();
    }

    public sealed override string ToILString() => $"{Capability.ToILString()} {BareType.ToILString()}";
}
