using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A type that has a capability applied.
/// </summary>
public abstract class CapabilityType : NonEmptyType
{
    /// <summary>
    /// Create a reference type for a class.
    /// </summary>
    public static CapabilityType<ObjectType> CreateClass(
        Capability capability,
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name)
        => Create(capability, ObjectType.CreateClass(containingPackage, containingNamespace, isAbstract, isConst, name),
            FixedList.Empty<DataType>());

    /// <summary>
    /// Create a reference type for a trait.
    /// </summary>
    public static CapabilityType<ObjectType> CreateTrait(
        Capability capability,
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name)
        => Create(capability, ObjectType.CreateTrait(containingPackage, containingNamespace, isConst, name),
            FixedList.Empty<DataType>());

    /// <summary>
    /// Create a object type for a given class or trait.
    /// </summary>
    public static CapabilityType<ObjectType> Create(
        Capability capability,
        ObjectType declaredType,
        IFixedList<DataType> typeArguments)
        => Create(capability, BareType.Create(declaredType, typeArguments));

    /// <summary>
    /// Create a object type for a given bare type.
    /// </summary>
    public static CapabilityType<ObjectType> Create(Capability capability, BareReferenceType<ObjectType> bareType)
        => CapabilityType<ObjectType>.Create(capability, bareType);

    /// <summary>
    /// Create an `Any` type for a given bare type.
    /// </summary>
    public static CapabilityType<AnyType> Create(Capability capability, BareReferenceType<AnyType> bareType)
        => CapabilityType<AnyType>.Create(capability, bareType);

    public Capability Capability { get; }
    public bool IsReadOnlyReference => Capability == Capability.Read;
    public bool IsConstantReference => Capability == Capability.Constant;
    public bool IsTemporarilyConstantReference => Capability == Capability.TemporarilyConstant;
    public bool IsIsolatedReference => Capability == Capability.Isolated;
    public bool IsTemporarilyIsolatedReference => Capability == Capability.TemporarilyIsolated;
    public bool IsIdentityReference => Capability == Capability.Identity;

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
    public bool AllowsMove => Capability.AllowsMove && !BareType.IsDeclaredConst;

    /// <summary>
    /// Does this capability allow a reference with it to be frozen to const if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsFreeze => Capability.AllowsFreeze;

    public abstract BareType BareType { get; }

    public virtual DeclaredType DeclaredType => BareType.DeclaredType;

    public IdentifierName? ContainingPackage => DeclaredType.ContainingPackage;

    public NamespaceName ContainingNamespace => DeclaredType.ContainingNamespace;

    public TypeName Name => DeclaredType.Name;

    public IFixedList<DataType> TypeArguments => BareType.GenericTypeArguments;

    public sealed override bool AllowsVariance => BareType.AllowsVariance;

    public sealed override bool HasIndependentTypeArguments => BareType.HasIndependentTypeArguments;

    public IFixedSet<BareReferenceType> Supertypes => BareType.Supertypes;

    public sealed override bool IsFullyKnown => BareType.IsFullyKnown;

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// `const`.
    /// </summary>
    public bool IsDeclaredConst => DeclaredType.IsDeclaredConst;

    private protected CapabilityType(Capability capability)
    {
        Capability = capability;
    }

    public sealed override IMaybeExpressionAntetype ToAntetype() => BareType.ToAntetype();

    public override DataType ReplaceTypeParametersIn(DataType type)
        => BareType.ReplaceTypeParametersIn(type);

    public override Pseudotype ReplaceTypeParametersIn(Pseudotype pseudotype)
        => BareType.ReplaceTypeParametersIn(pseudotype);

    public abstract CapabilityType With(Capability capability);

    /// <remarks>For constant types, there can still be read only references. For example, inside
    /// the constructor.</remarks>
    public override CapabilityType WithoutWrite() => With(Capability.WithoutWrite());

    public override Type AccessedVia(ICapabilityConstraint capability)
    {
        switch (capability)
        {
            case Capability c:
                var newCapability = Capability.AccessedVia(c);
                var newBareType = BareType.AccessedVia(c);
                if (ReferenceEquals(newBareType, BareType))
                    return ReferenceEquals(newCapability, Capability) ? this : With(newCapability);

                return newBareType.With(newCapability);
            case CapabilitySet c:
                return new SelfViewpointType(c, this);
            default:
                throw ExhaustiveMatch.Failed(capability);
        }
    }

    public CapabilityType UpcastTo(DeclaredType target)
    {
        if (DeclaredType.Equals(target))
            return this;

        var supertype = Supertypes.Where(s => s.DeclaredType == target).TrySingle();
        if (supertype is null)
            throw new ArgumentException($"The type {target} is not a supertype of {this}.");

        return supertype.With(Capability);
    }

    #region Equality
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilityType otherType
               && Capability == otherType.Capability
               && BareType.Equals(otherType.BareType);
    }

    public override int GetHashCode()
        => HashCode.Combine(Capability, BareType);
    #endregion

    public sealed override string ToSourceCodeString()
    {
        if (Capability == Capability.Read || (Capability == Capability.Constant && DeclaredType.IsDeclaredConst))
            return BareType.ToSourceCodeString();

        return $"{Capability.ToSourceCodeString()} {BareType.ToSourceCodeString()}";

    }

    public sealed override string ToILString()
        => $"{Capability.ToILString()} {BareType.ToILString()}";
}

public sealed class CapabilityType<TDeclared> : CapabilityType
    where TDeclared : DeclaredType
{
    public static CapabilityType<TDeclaredReferenceType> Create<TDeclaredReferenceType>(
        Capability capability,
        BareReferenceType<TDeclaredReferenceType> bareType)
        where TDeclaredReferenceType : DeclaredReferenceType, TDeclared
        => new(capability, bareType);

    public static CapabilityType<TDeclaredValueType> Create<TDeclaredValueType>(
        Capability capability,
        BareValueType<TDeclaredValueType> bareType)
        where TDeclaredValueType : DeclaredValueType, TDeclared
        => new(capability, bareType);

    public override BareType BareType { get; }

    public override TDeclared DeclaredType => (TDeclared)BareType.DeclaredType;

    private CapabilityType(Capability capability, BareType bareType)
        : base(capability)
    {
        if (typeof(TDeclared).IsAbstract)
            throw new ArgumentException($"The type parameter must be a concrete {nameof(DeclaredType)}.", nameof(TDeclared));

        BareType = bareType;
    }

    public override CapabilityType<TDeclared> With(Capability capability)
    {
        if (capability == Capability) return this;
        return new(capability, BareType);
    }
}
