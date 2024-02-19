using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public abstract class ReferenceType : CapabilityType
{
    public abstract override BareReferenceType BareType { get; }

    public override DeclaredReferenceType DeclaredType => BareType.DeclaredType;

    /// <summary>
    /// Create a reference type for a class.
    /// </summary>
    public static ReferenceType<ObjectType> CreateClass(
        ReferenceCapability capability,
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name)
        => Create(capability,
            ObjectType.CreateClass(containingPackage, containingNamespace, isAbstract, isConst, name),
            FixedList.Empty<DataType>());

    /// <summary>
    /// Create a reference type for a trait.
    /// </summary>
    public static ReferenceType<ObjectType> Create(
        ReferenceCapability capability,
        SimpleName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name)
        => Create(capability,
            ObjectType.CreateTrait(containingPackage, containingNamespace, isConst, name),
            FixedList.Empty<DataType>());

    /// <summary>
    /// Create a object type for a given class or trait.
    /// </summary>
    public static ReferenceType<ObjectType> Create(
        ReferenceCapability capability,
        ObjectType declaredType,
        IFixedList<DataType> typeArguments)
        => Create(capability, Bare.BareType.Create(declaredType, typeArguments));

    /// <summary>
    /// Create a object type for a given bare type.
    /// </summary>
    public static ReferenceType<ObjectType> Create(
        ReferenceCapability capability, BareReferenceType<ObjectType> bareType)
        => new(capability, bareType);

    /// <summary>
    /// Create an `Any` type for a given bare type.
    /// </summary>
    public static ReferenceType<AnyType> Create(
        ReferenceCapability capability, BareReferenceType<AnyType> bareType)
        => new(capability, bareType);

    private protected ReferenceType(ReferenceCapability capability)
        : base(capability)
    {
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
        if (typeof(TDeclared).IsAbstract)
            throw new ArgumentException($"The type parameter must be a concrete {nameof(DeclaredReferenceType)}.", nameof(TDeclared));
        BareType = bareType;
    }

    public override ReferenceType<TDeclared> With(ReferenceCapability referenceCapability)
    {
        if (referenceCapability == Capability) return this;
        return new(referenceCapability, BareType);
    }
}
