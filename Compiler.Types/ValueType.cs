using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public abstract class ValueType : CapabilityType
{
    public abstract override BareValueType BareType { get; }

    public override DeclaredValueType DeclaredType => BareType.DeclaredType;

    private protected ValueType(ReferenceCapability capability)
        : base(capability) { }

    #region Equality
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ValueType otherType && BareType == otherType.BareType;
    }

    public override int GetHashCode() => HashCode.Combine(BareType);
    #endregion
}

public sealed class ValueType<TDeclared> : ValueType
    where TDeclared : DeclaredValueType
{
    public override BareValueType<TDeclared> BareType { get; }

    public override TDeclared DeclaredType => BareType.DeclaredType;

    internal ValueType(ReferenceCapability capability, BareValueType<TDeclared> bareType)
        : base(capability)
    {
        if (typeof(TDeclared).IsAbstract)
            throw new ArgumentException($"The type parameter must be a concrete {nameof(DeclaredValueType)}.", nameof(TDeclared));
        BareType = bareType;
    }
}
