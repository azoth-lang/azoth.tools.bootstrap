using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

// TODO should `Any` be a plain type or a type constructor? It seems it could just be listed as a supertype
public sealed class AnyAntetype : NonGenericNominalAntetype, INonVoidAntetype
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

    public override IFixedSet<NamedPlainType> Supertypes => FixedSet.Empty<NamedPlainType>();

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
        // AnyAntetype is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(AnyAntetype));
    #endregion

    public override string ToString() => Name.ToString();
}
