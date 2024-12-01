using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class AnyAntetype : NonGenericNominalAntetype, INonVoidAntetype, IDeclaredAntetype
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
    public override bool CanBeConstructed => false;

    public override SpecialTypeName Name => SpecialTypeName.Any;

    IFixedList<AntetypeGenericParameter> IDeclaredAntetype.GenericParameters
        => FixedList.Empty<AntetypeGenericParameter>();

    IFixedList<GenericParameterAntetype> IDeclaredAntetype.GenericParameterAntetypes
        => FixedList.Empty<GenericParameterAntetype>();

    public override IFixedSet<NominalAntetype> Supertypes => FixedSet.Empty<NominalAntetype>();

    public bool HasReferenceSemantics => true;

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
        // AnyAntetype is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(AnyAntetype));
    #endregion

    public override string ToString() => Name.ToString();
}
