using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class OrdinaryAssociatedPlainType : AssociatedPlainType
{
    public override StandardName Name { get; }

    public override IFixedSet<ConstructedPlainType> Supertypes => AnyTypeConstructor.Set;

    public OrdinaryAssociatedPlainType(OrdinaryTypeConstructor containingType, StandardName name)
        : base(containingType)
    {
        Name = name;
    }

    #region Equality
    public override bool Equals(IMaybePlainType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OrdinaryAssociatedPlainType otherType
            && ContainingType.Equals(otherType.ContainingType)
            && Name.Equals(otherType.Name);
    }

    public override int GetHashCode() => HashCode.Combine(ContainingType, Name);
    #endregion
}
