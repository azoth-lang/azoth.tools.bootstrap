using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public class AnyAntetype : NonGenericNominalAntetype, INonVoidAntetype, IDeclaredAntetype
{
    #region Singleton
    internal static readonly AnyAntetype Instance = new();

    private AnyAntetype()
    {
    }
    #endregion

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
