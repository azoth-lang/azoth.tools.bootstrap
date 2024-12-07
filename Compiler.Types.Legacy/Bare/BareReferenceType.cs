using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

/// <summary>
/// A reference type without a reference capability.
/// </summary>
public sealed class BareReferenceType : BareNonVariableType
{
    public override DeclaredReferenceType DeclaredType { get; }

    internal BareReferenceType(DeclaredReferenceType declaredType, IFixedList<IType> genericTypeArguments)
        : base(declaredType, genericTypeArguments)
    {
        DeclaredType = declaredType;
    }

    public override BareReferenceType AccessedVia(Capability capability)
    {
        if (!HasIndependentTypeArguments) return this;
        var newTypeArguments = TypeArgumentsAccessedVia(capability);
        if (ReferenceEquals(newTypeArguments, GenericTypeArguments)) return this;
        return new(DeclaredType, newTypeArguments);
    }

    public override BareReferenceType With(IFixedList<IType> typeArguments)
        => new(DeclaredType, typeArguments);

    public override CapabilityType With(Capability capability) => CapabilityType.Create(capability, this);

    #region Equality

    public override bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BareReferenceType otherType
               && DeclaredType == otherType.DeclaredType
               && GenericTypeArguments.Equals(otherType.GenericTypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaredType, GenericTypeArguments);
    #endregion
}
