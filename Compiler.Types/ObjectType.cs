using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// Object types are the types created with class and trait declarations. An
/// object type may have generic parameters that may be filled with generic
/// arguments. An object type with generic parameters but no generic arguments
/// is an *unbound type*. One with generic arguments supplied for all
/// parameters is *a constructed type*. One with some but not all arguments
/// supplied is *partially constructed type*.
/// </summary>
/// <remarks>
/// There will be two special object types `Type` and `Metatype`
/// </remarks>
public sealed class ObjectType : ReferenceType
{
    public override BareObjectType BareType { get; }

    public override DeclaredReferenceType DeclaredType => BareType.DeclaredType;

    public override bool HasIndependentTypeArguments => BareType.HasIndependentTypeArguments;

    internal ObjectType(
        ReferenceCapability capability,
        BareObjectType bareType)
        : base(capability, bareType)
    {
        BareType = bareType;
    }

    public override ObjectType With(ReferenceCapability referenceCapability)
    {
        if (referenceCapability == Capability) return this;
        return new(referenceCapability, BareType);
    }

    /// <remarks>For constant types, there can still be read only references. For example, inside
    /// the constructor.</remarks>
    public override ObjectType WithoutWrite() => (ObjectType)base.WithoutWrite();

    public override DataType ReplaceTypeParametersIn(DataType type)
        => BareType.ReplaceTypeParametersIn(type);

    public override Pseudotype ReplaceTypeParametersIn(Pseudotype pseudotype)
        => BareType.ReplaceTypeParametersIn(pseudotype);

    #region Equality
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ObjectType otherType
               && Capability == otherType.Capability
               && BareType == otherType.BareType;
    }

    public override int GetHashCode() => HashCode.Combine(Capability, BareType);
    #endregion
}
