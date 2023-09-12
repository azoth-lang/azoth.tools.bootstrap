using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The universal type all reference types can be converted to. A top type
/// for reference and function types.
/// </summary>
public sealed class AnyType : ReferenceType
{
    public AnyType(ReferenceCapability capability)
        : base(capability, DeclaredAnyType.Instance)
    {
    }

    public override SpecialTypeName Name => SpecialTypeName.Any;

    public override DeclaredAnyType DeclaredType => DeclaredAnyType.Instance;

    public override bool IsFullyKnown => true;

    #region Equals
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is AnyType otherType
               && Capability == otherType.Capability;
    }

    public override int GetHashCode() => HashCode.Combine(SpecialTypeName.Any, Capability);
    #endregion

    public override AnyType To(ReferenceCapability referenceCapability)
        => new(referenceCapability);

    public override string ToILString() => $"{Capability.ToILString()} Any";

    public override string ToSourceCodeString()
        => Capability != ReferenceCapability.ReadOnly
            ? $"{Capability.ToSourceString()} Any" : "Any";
}
