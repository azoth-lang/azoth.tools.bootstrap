using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The type variable referred to with the `Self` type variable.
/// </summary>
public sealed class SelfPlainType : AssociatedPlainType
{
    public override TypeName Name => BuiltInTypeName.Self;
    public override IFixedSet<ConstructedPlainType> Supertypes { get; }

    public SelfPlainType(TypeConstructor containingType)
        : base(containingType)
    {
        Supertypes = containingType.Supertypes.Select(s => s.PlainType)
                                   .Append(containingType.ConstructWithParameterPlainTypes())
                                   .ToFixedSet();
    }

    public CapabilitySetSelfType With(CapabilitySet capability) => new(capability, this);

    #region Equality
    public override bool Equals(IMaybePlainType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SelfPlainType otherType
               && ContainingType.Equals(otherType.ContainingType);
    }

    public override int GetHashCode() => HashCode.Combine(typeof(SelfPlainType), ContainingType);
    #endregion
}
