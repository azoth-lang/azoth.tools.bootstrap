using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

/// <summary>
/// A type that has a capability applied.
/// </summary>
public sealed class CapabilityType : NonEmptyType, INonVoidType
{
    /// <summary>
    /// Create a reference type for a class.
    /// </summary>
    public static CapabilityType CreateClass(
        Capability capability,
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name)
        => Create(capability, OrdinaryDeclaredType.CreateClass(containingPackage, containingNamespace, isAbstract, isConst, name), []);

    /// <summary>
    /// Create a reference type for a trait.
    /// </summary>
    public static CapabilityType CreateTrait(
        Capability capability,
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name)
        => Create(capability, OrdinaryDeclaredType.CreateTrait(containingPackage, containingNamespace, isConst, name), []);

    /// <summary>
    /// Create an object type for a given class or trait.
    /// </summary>
    public static CapabilityType Create(
        Capability capability,
        OrdinaryDeclaredType declaredType,
        IFixedList<IType> typeArguments)
        => Create(capability, BareNonVariableType.Create(declaredType, typeArguments));

    public static CapabilityType Create(Capability capability, BareNonVariableType bareType)
        => new(capability, bareType);

    public Capability Capability { get; }
    public bool IsReadOnlyReference => Capability == Capability.Read;
    public bool IsConstantReference => Capability == Capability.Constant;
    public bool IsTemporarilyConstantReference => Capability == Capability.TemporarilyConstant;
    public bool IsIsolatedReference => Capability == Capability.Isolated;
    public bool IsTemporarilyIsolatedReference => Capability == Capability.TemporarilyIsolated;
    public bool IsIdentityReference => Capability == Capability.Identity;

    public bool AllowsInit => Capability.AllowsInit;

    public bool AllowsWrite => Capability.AllowsWrite;

    /// <summary>
    /// Does this capability allow a reference with it to be moved if reference sharing permits.
    /// </summary>
    public bool AllowsMove => Capability.AllowsMove && !BareType.IsDeclaredConst;

    /// <summary>
    /// Does this capability allow a reference with it to be frozen to const if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsFreeze => Capability.AllowsFreeze;

    public BareNonVariableType BareType { get; }

    public TypeSemantics Semantics => DeclaredType.Semantics;

    public DeclaredType DeclaredType => BareType.TypeConstructor;

    public IdentifierName? ContainingPackage => DeclaredType.ContainingPackage;

    public NamespaceName ContainingNamespace => DeclaredType.ContainingNamespace;

    public TypeName Name => DeclaredType.Name;

    public IFixedList<IMaybeType> TypeArguments => BareType.TypeArguments;

    public override bool AllowsVariance => BareType.AllowsVariance;

    public override bool HasIndependentTypeArguments => BareType.HasIndependentTypeArguments;

    public IFixedSet<BareNonVariableType> Supertypes => BareType.Supertypes;

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// `const`.
    /// </summary>
    public bool IsDeclaredConst => DeclaredType.IsDeclaredConst;

    private CapabilityType(Capability capability, BareNonVariableType bareType)
    {
        Capability = capability;
        BareType = bareType;
    }

    public sealed override INonVoidPlainType ToPlainType() => BareType.ToPlainType();

    public override IType ReplaceTypeParametersIn(IType type)
        => BareType.ReplaceTypeParametersIn(type);

    public override IMaybeType ReplaceTypeParametersIn(IMaybeType type)
        => BareType.ReplaceTypeParametersIn(type);

    public override IPseudotype ReplaceTypeParametersIn(IPseudotype pseudotype)
        => BareType.ReplaceTypeParametersIn(pseudotype);

    public override IMaybePseudotype ReplaceTypeParametersIn(IMaybePseudotype pseudotype)
        => BareType.ReplaceTypeParametersIn(pseudotype);

    public CapabilityType With(Capability capability)
    {
        if (capability == Capability) return this;
        return new(capability, BareType);
    }

    /// <remarks>For constant types, there can still be read only references. For example, inside
    /// the constructor.</remarks>
    public override CapabilityType WithoutWrite() => With(Capability.WithoutWrite());
    IMaybeNonVoidType IMaybeNonVoidType.WithoutWrite() => WithoutWrite();

    IMaybeType IMaybeType.AccessedVia(IMaybePseudotype contextType) => (IMaybeType)AccessedVia(contextType);

    public override IType AccessedVia(ICapabilityConstraint capability)
    {
        switch (capability)
        {
            case Capability c:
                var newCapability = Capability.AccessedVia(c);
                var newBareType = BareType.AccessedVia(c); // Access can affect type arguments
                if (ReferenceEquals(newBareType, BareType))
                    return ReferenceEquals(newCapability, Capability) ? this : With(newCapability);

                return newBareType.With(newCapability);
            case CapabilitySet c:
                return SelfViewpointType.Create(c, this);
            default:
                throw ExhaustiveMatch.Failed(capability);
        }
    }
    IMaybeType IMaybeType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);

    public CapabilityType UpcastTo(DeclaredType target)
    {
        if (DeclaredType.Equals(target))
            return this;

        // TODO this will fail if the type implements the target type in multiple ways.
        var supertype = Supertypes.Where(s => s.TypeConstructor == target).TrySingle();
        if (supertype is null)
            throw new ArgumentException($"The type {target} is not a supertype of {this}.");

        return supertype.With(Capability);
    }

    #region Equality
    public override bool Equals(IMaybeType? other)
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
