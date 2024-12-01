using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The type variable referred to with the `Self` type variable.
/// </summary>
public sealed class SelfAntetype : NonGenericNominalAntetype, INonVoidAntetype
{
    /// <summary>
    /// As a type variable, a `Self` type cannot be constructed.
    /// </summary>
    public override bool CanBeInstantiated => false;
    public IOrdinaryTypeConstructor ContainingType { get; }
    public override TypeName Name => SpecialTypeName.Self;
    public override IFixedSet<NominalAntetype> Supertypes { get; }

    public SelfAntetype(IOrdinaryTypeConstructor containingType)
    {
        ContainingType = containingType;
        Supertypes = containingType.Supertypes.Append(containingType.ConstructWithGenericParameterPlayTypes()).ToFixedSet();
    }

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SelfAntetype otherType
               && ContainingType.Equals(otherType.ContainingType);
    }

    public override int GetHashCode() => HashCode.Combine(typeof(SelfAntetype), ContainingType);
    #endregion

    public override string ToString() => $"{ContainingType}.Self";
}
