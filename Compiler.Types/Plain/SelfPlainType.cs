using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The type variable referred to with the `Self` type variable.
/// </summary>
public sealed class SelfPlainType : VariablePlainType
{
    public OrdinaryTypeConstructor ContainingType { get; }
    public override TypeName Name => SpecialTypeName.Self;
    public override IFixedSet<NamedPlainType> Supertypes { get; }

    public SelfPlainType(OrdinaryTypeConstructor containingType)
    {
        ContainingType = containingType;
        Supertypes = containingType.Supertypes.Append(containingType.ConstructWithGenericParameterPlainTypes()).ToFixedSet();
    }

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

    public override string ToString() => $"{ContainingType}.Self";
}
