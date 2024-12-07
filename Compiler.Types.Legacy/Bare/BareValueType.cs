using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

/// <summary>
/// A value type without a reference capability.
/// </summary>
public sealed class BareValueType : BareNonVariableType
{
    public override DeclaredValueType DeclaredType { get; }

    internal BareValueType(DeclaredValueType declaredType, IFixedList<IType> typeArguments)
        : base(declaredType, typeArguments)
    {
        DeclaredType = declaredType;
    }

    public override BareValueType AccessedVia(Capability capability)
    {
        if (!HasIndependentTypeArguments) return this;
        var newTypeArguments = TypeArgumentsAccessedVia(capability);
        if (ReferenceEquals(newTypeArguments, GenericTypeArguments)) return this;
        return new(DeclaredType, newTypeArguments);
    }

    public override BareValueType With(IFixedList<IType> typeArguments)
        => new(DeclaredType, typeArguments);

    public override CapabilityType With(Capability capability) => CapabilityType.Create(capability, this);

    #region Equality
    public override bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BareValueType otherType
               && DeclaredType == otherType.DeclaredType
               && GenericTypeArguments.Equals(otherType.GenericTypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaredType, GenericTypeArguments);
    #endregion
}
