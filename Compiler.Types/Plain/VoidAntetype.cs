using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The void type behaves similar to a unit type. However, it represents the
/// lack of a value. For example, a function returning `void` doesn't return
/// a value. A parameter of type `void` is dropped from the parameter list.
/// </summary>
public sealed class VoidAntetype : EmptyAntetype
{
    #region Singleton
    internal static readonly VoidAntetype Instance = new VoidAntetype();

    private VoidAntetype()
        : base(SpecialTypeName.Void) { }
    #endregion

    public override IFixedSet<NominalAntetype> Supertypes => FixedSet.Empty<NominalAntetype>();

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
        // VoidAntetype is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(VoidAntetype));
    #endregion
}
