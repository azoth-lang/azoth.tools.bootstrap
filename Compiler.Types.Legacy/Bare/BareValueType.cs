using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

/// <summary>
/// A value type without a reference capability.
/// </summary>
public abstract class BareValueType : BareNonVariableType
{
    public abstract override DeclaredValueType DeclaredType { get; }

    private protected BareValueType(DeclaredType declaredType, IFixedList<IType> typeArguments)
        : base(declaredType, typeArguments) { }

    public abstract override BareValueType AccessedVia(Capability capability);

    public abstract override BareValueType With(IFixedList<IType> typeArguments);

    public abstract override CapabilityType With(Capability capability);
}

public sealed class BareValueType<TDeclared> : BareValueType
    where TDeclared : DeclaredValueType
{
    public override TDeclared DeclaredType { get; }

    internal BareValueType(TDeclared declaredType, IFixedList<IType> typeArguments)
        : base(declaredType, typeArguments)
    {
        if (typeof(TDeclared).IsAbstract)
            throw new ArgumentException($"The type parameter must be a concrete {nameof(DeclaredValueType)}.", nameof(TDeclared));
        DeclaredType = declaredType;
    }

    public override BareValueType<TDeclared> AccessedVia(Capability capability)
    {
        if (!HasIndependentTypeArguments) return this;
        var newTypeArguments = TypeArgumentsAccessedVia(capability);
        if (ReferenceEquals(newTypeArguments, GenericTypeArguments)) return this;
        return new(DeclaredType, newTypeArguments);
    }

    public override BareValueType<TDeclared> With(IFixedList<IType> typeArguments)
        => new(DeclaredType, typeArguments);

    public override CapabilityType With(Capability capability)
        => CapabilityType.Create(capability, this);

    #region Equality
    public override bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BareValueType<TDeclared> otherType
               && DeclaredType == otherType.DeclaredType
               && GenericTypeArguments.Equals(otherType.GenericTypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaredType, GenericTypeArguments);
    #endregion
}
