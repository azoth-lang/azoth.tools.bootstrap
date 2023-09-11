using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The universal type all reference types can be converted to. A top type
/// for reference and function types.
/// </summary>
/// <remarks>
/// `Any` is "declared" mutable so that it can hold mutable references to
/// mutable types.
/// </remarks>
public sealed class AnyType : ReferenceType
{
    public AnyType(ReferenceCapability capability)
        : base(capability, DeclaredAnyType.Instance)
    {
    }

    public new DeclaredAnyType DeclaredType => (DeclaredAnyType)base.DeclaredType;

    public override bool IsKnown => true;

    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is AnyType otherType
               && Capability == otherType.Capability;
    }

    public override int GetHashCode() => HashCode.Combine(SpecialTypeName.Any, Capability);

    public override AnyType To(ReferenceCapability referenceCapability)
        => new(referenceCapability);

    public override string ToILString() => $"{Capability.ToILString()} Any";

    public override string ToSourceCodeString()
        => Capability != ReferenceCapability.ReadOnly
            ? $"{Capability.ToSourceString()} Any" : "Any";
}
