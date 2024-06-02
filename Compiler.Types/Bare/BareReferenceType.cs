using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

/// <summary>
/// An reference type without a reference capability.
/// </summary>
public abstract class BareReferenceType : BareType
{
    public abstract override DeclaredReferenceType DeclaredType { get; }

    private protected BareReferenceType(DeclaredReferenceType declaredType, IFixedList<DataType> genericTypeArguments)
        : base(declaredType, genericTypeArguments)
    {
    }

    public abstract override BareReferenceType AccessedVia(Capability capability);

    public abstract override BareReferenceType With(IFixedList<DataType> typeArguments);

    public abstract override CapabilityType With(Capability capability);
}

public sealed class BareReferenceType<TDeclared> : BareReferenceType
    where TDeclared : DeclaredReferenceType
{
    public override TDeclared DeclaredType { get; }

    internal BareReferenceType(TDeclared declaredType, IFixedList<DataType> genericTypeArguments)
        : base(declaredType, genericTypeArguments)
    {
        if (typeof(TDeclared).IsAbstract)
            throw new ArgumentException($"The type parameter must be a concrete {nameof(DeclaredReferenceType)}.", nameof(TDeclared));
        DeclaredType = declaredType;
    }

    public override BareReferenceType<TDeclared> AccessedVia(Capability capability)
    {
        if (!HasIndependentTypeArguments) return this;
        var newTypeArguments = TypeArgumentsAccessedVia(capability);
        if (ReferenceEquals(newTypeArguments, GenericTypeArguments)) return this;
        return new(DeclaredType, newTypeArguments);
    }

    public override BareReferenceType<TDeclared> With(IFixedList<DataType> typeArguments)
        => new(DeclaredType, typeArguments);

    public override CapabilityType<TDeclared> With(Capability capability)
        => CapabilityType<TDeclared>.Create(capability, this);

    #region Equality
    public override bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BareReferenceType<TDeclared> otherType
               && DeclaredType == otherType.DeclaredType
               && GenericTypeArguments.ItemsEqual(otherType.GenericTypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaredType, GenericTypeArguments);
    #endregion
}
