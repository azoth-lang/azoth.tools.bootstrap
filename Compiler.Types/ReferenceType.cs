using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
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
        Capability capability,
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
        Capability capability,
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
        Capability capability,
        ObjectType declaredType,
        IFixedList<DataType> typeArguments)
        => Create(capability, Bare.BareType.Create(declaredType, typeArguments));

    /// <summary>
    /// Create a object type for a given bare type.
    /// </summary>
    public static ReferenceType<ObjectType> Create(
        Capability capability, BareReferenceType<ObjectType> bareType)
        => new(capability, bareType);

    /// <summary>
    /// Create an `Any` type for a given bare type.
    /// </summary>
    public static ReferenceType<AnyType> Create(
        Capability capability, BareReferenceType<AnyType> bareType)
        => new(capability, bareType);

    private protected ReferenceType(Capability capability)
        : base(capability)
    {
    }

    /// <remarks>For constant types, there can still be read only references. For example, inside
    /// the constructor.</remarks>
    public override ReferenceType WithoutWrite() => With(Capability.WithoutWrite());

    public override DataType AccessedVia(ICapabilityConstraint capability)
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

    public abstract override ReferenceType With(Capability capability);

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
}

public sealed class ReferenceType<TDeclared> : ReferenceType
    where TDeclared : DeclaredReferenceType
{
    public override BareReferenceType<TDeclared> BareType { get; }

    public override TDeclared DeclaredType => BareType.DeclaredType;

    internal ReferenceType(Capability capability, BareReferenceType<TDeclared> bareType)
        : base(capability)
    {
        if (typeof(TDeclared).IsAbstract)
            throw new ArgumentException($"The type parameter must be a concrete {nameof(DeclaredReferenceType)}.", nameof(TDeclared));
        BareType = bareType;
    }

    public override ReferenceType<TDeclared> With(Capability capability)
    {
        if (capability == Capability) return this;
        return new(capability, BareType);
    }
}
