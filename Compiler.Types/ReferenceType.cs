using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public abstract class ReferenceType : NonEmptyType
{
    public ReferenceCapability Capability { get; }
    public bool IsReadOnlyReference => Capability == ReferenceCapability.Read;
    public bool IsConstantReference => Capability == ReferenceCapability.Constant;
    public bool IsTemporarilyConstantReference => Capability == ReferenceCapability.TemporarilyConstant;
    public bool IsIsolatedReference => Capability == ReferenceCapability.Isolated;
    public bool IsTemporarilyIsolatedReference => Capability == ReferenceCapability.TemporarilyIsolated;
    public bool IsIdentityReference => Capability == ReferenceCapability.Identity;

    public bool AllowsInit => Capability.AllowsInit;

    public override bool AllowsWrite => Capability.AllowsWrite;

    public override bool AllowsWriteAliases => Capability.AllowsWriteAliases;

    /// <summary>
    /// Does this reference allow it to be recovered to isolated if reference sharing permits.
    /// </summary>
    public bool AllowsRecoverIsolation => Capability.AllowsRecoverIsolation;

    /// <summary>
    /// Does this capability allow a reference with it to be moved if reference sharing permits.
    /// </summary>
    public bool AllowsMove => Capability.AllowsMove && !BareType.IsConstType;

    /// <summary>
    /// Does this capability allow a reference with it to be frozen to const if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsFreeze => Capability.AllowsFreeze;

    public abstract BareReferenceType BareType { get; }

    public virtual DeclaredReferenceType DeclaredType => BareType.DeclaredType;

    public FixedList<DataType> TypeArguments => BareType.TypeArguments;

    public FixedSet<BareReferenceType> Supertypes => BareType.Supertypes;

    public override bool IsFullyKnown => BareType.IsFullyKnown;

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// `const`.
    /// </summary>
    public bool IsDeclaredConstant => DeclaredType.IsConstType;

    public SimpleName? ContainingPackage => DeclaredType.ContainingPackage;
    public NamespaceName ContainingNamespace => DeclaredType.ContainingNamespace;

    public TypeName Name => DeclaredType.Name;

    public override TypeSemantics Semantics => TypeSemantics.Reference;

    public override bool HasIndependentTypeArguments => BareType.HasIndependentTypeArguments;

    /// <summary>
    /// Create a object type for a given class or trait.
    /// </summary>
    public static ReferenceType<DeclaredObjectType> Create(
        ReferenceCapability capability,
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        bool isClass,
        string name)
        => Create(capability,
            DeclaredObjectType.Create(containingPackage, containingNamespace, isAbstract, isConst, isClass, name),
            FixedList<DataType>.Empty);

    /// <summary>
    /// Create a object type for a given class or trait.
    /// </summary>
    public static ReferenceType<DeclaredObjectType> Create(
        ReferenceCapability capability,
        DeclaredObjectType declaredType,
        FixedList<DataType> typeArguments)
        => Create(capability, BareReferenceType.Create(declaredType, typeArguments));

    /// <summary>
    /// Create a object type for a given bare type.
    /// </summary>
    public static ReferenceType<DeclaredObjectType> Create(ReferenceCapability capability, BareReferenceType<DeclaredObjectType> bareType)
        => new(capability, bareType);

    /// <summary>
    /// Create an `Any` type for a given bare type.
    /// </summary>
    public static ReferenceType<DeclaredAnyType> Create(ReferenceCapability capability, BareReferenceType<DeclaredAnyType> bareType)
        => new(capability, bareType);

    protected ReferenceType(ReferenceCapability capability)
    {
        Capability = capability;
    }

    /// <remarks>For constant types, there can still be read only references. For example, inside
    /// the constructor.</remarks>
    public override ReferenceType WithoutWrite() => With(Capability.WithoutWrite());

    public override DataType AccessedVia(IReferenceCapabilityConstraint capability)
    {
        switch (capability)
        {
            case ReferenceCapability c:
                var newCapability = Capability.AccessedVia(c);
                var bareType = BareType.AccessedVia(c);
                if (ReferenceEquals(bareType, BareType))
                    return With(newCapability);

                return bareType.With(newCapability);
            case ReferenceCapabilityConstraint c:
                return new SelfViewpointType(c, this);
            default:
                throw ExhaustiveMatch.Failed(capability);
        }
    }

    public abstract ReferenceType With(ReferenceCapability referenceCapability);

    public override DataType ReplaceTypeParametersIn(DataType type)
        => BareType.ReplaceTypeParametersIn(type);

    public override Pseudotype ReplaceTypeParametersIn(Pseudotype pseudotype)
        => BareType.ReplaceTypeParametersIn(pseudotype);

    #region Equality
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ReferenceType otherType
               && Capability == otherType.Capability
               && BareType == otherType.BareType;
    }

    public override int GetHashCode()
        => HashCode.Combine(Capability, BareType);
    #endregion

    public override string ToSourceCodeString()
    {
        if (Capability != ReferenceCapability.Read)
            return $"{Capability.ToSourceString()} {BareType.ToSourceCodeString()}";

        return BareType.ToSourceCodeString();
    }

    public override string ToILString() => $"{Capability.ToILString()} {BareType.ToILString()}";
}

public sealed class ReferenceType<TDeclared> : ReferenceType
    where TDeclared : DeclaredReferenceType
{
    public override BareReferenceType<TDeclared> BareType { get; }

    public override TDeclared DeclaredType => BareType.DeclaredType;

    internal ReferenceType(ReferenceCapability capability, BareReferenceType<TDeclared> bareType)
        : base(capability)
    {
        BareType = bareType;
    }

    public override ReferenceType<TDeclared> With(ReferenceCapability referenceCapability)
    {
        if (referenceCapability == Capability) return this;
        return new(referenceCapability, BareType);
    }
}
