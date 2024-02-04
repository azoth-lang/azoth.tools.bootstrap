using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The universal type all reference types can be converted to. A top type
/// for reference and function types.
/// </summary>
public sealed class AnyType : ReferenceType
{
    public AnyType(ReferenceCapability capability)
        : base(capability, BareAnyType.Instance)
    {
    }

    public override SpecialTypeName Name => SpecialTypeName.Any;

    public override BareAnyType BareType => BareAnyType.Instance;

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

    public override int GetHashCode() => HashCode.Combine(Capability, SpecialTypeName.Any);
    #endregion

    public override AnyType With(ReferenceCapability referenceCapability)
    {
        if (referenceCapability == Capability) return this;
        return new(referenceCapability);
    }
}
