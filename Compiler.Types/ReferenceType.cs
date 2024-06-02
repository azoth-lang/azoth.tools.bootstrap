using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public abstract class ReferenceType : CapabilityType
{
    private protected ReferenceType(Capability capability)
        : base(capability)
    {
    }

    public abstract override ReferenceType With(Capability capability);

    #region Equality
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ReferenceType otherType
               && Capability == otherType.Capability
               && BareType.Equals(otherType.BareType);
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
