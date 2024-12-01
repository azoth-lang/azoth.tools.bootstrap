using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class AnyAntetype : NonGenericNominalAntetype, INonVoidAntetype, ITypeConstructor
{
    #region Singleton
    internal static readonly AnyAntetype Instance = new();

    private AnyAntetype()
    {
    }
    #endregion

    /// <summary>
    /// The `Any` type cannot be constructed because it is abstract.
    /// </summary>
    public override bool CanBeInstantiated => false;

    public override SpecialTypeName Name => SpecialTypeName.Any;

    IFixedList<TypeConstructorParameter> ITypeConstructor.Parameters
        => FixedList.Empty<TypeConstructorParameter>();

    IFixedList<GenericParameterPlainType> ITypeConstructor.GenericParameterPlainTypes
        => FixedList.Empty<GenericParameterPlainType>();

    public override IFixedSet<NamedPlainType> Supertypes => FixedSet.Empty<NamedPlainType>();

    public bool HasReferenceSemantics => true;

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
        // AnyAntetype is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(AnyAntetype));
    #endregion

    public override string ToString() => Name.ToString();
}
