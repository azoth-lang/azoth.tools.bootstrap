using System;
using System.Linq;
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

    private protected BareReferenceType(DeclaredReferenceType declaredType, IFixedList<DataType> typeArguments)
        : base(declaredType, typeArguments)
    {
    }

    public abstract override BareReferenceType AccessedVia(ReferenceCapability capability);

    public abstract override BareReferenceType With(IFixedList<DataType> typeArguments);

    public abstract override ReferenceType With(ReferenceCapability capability);
}

public sealed class BareReferenceType<TDeclared> : BareReferenceType
    where TDeclared : DeclaredReferenceType
{
    public override TDeclared DeclaredType { get; }

    internal BareReferenceType(TDeclared declaredType, IFixedList<DataType> typeArguments)
        : base(declaredType, typeArguments)
    {
        if (typeof(TDeclared).IsAbstract)
            throw new ArgumentException($"The type parameter must be a concrete {nameof(DeclaredReferenceType)}.", nameof(TDeclared));
        DeclaredType = declaredType;
    }

    public override BareReferenceType<TDeclared> AccessedVia(ReferenceCapability capability)
    {
        if (DeclaredType.GenericParameters.All(p => p.Variance != Variance.Independent)) return this;
        var newTypeArguments = DeclaredType.GenericParameters.Zip(TypeArguments,
            (p, arg) => p.Variance == Variance.Independent ? arg.AccessedVia(capability) : arg).ToFixedList();
        return new(DeclaredType, newTypeArguments);
    }

    public override BareReferenceType<TDeclared> With(IFixedList<DataType> typeArguments)
        => new(DeclaredType, typeArguments);

    public override ReferenceType<TDeclared> With(ReferenceCapability capability)
        => new(capability, this);

    #region Equality
    public override bool Equals(BareType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BareReferenceType<TDeclared> otherType
               && DeclaredType == otherType.DeclaredType
               && TypeArguments.ItemsEquals(otherType.TypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaredType, TypeArguments);
    #endregion
}
