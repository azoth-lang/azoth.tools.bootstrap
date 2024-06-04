using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Compiler.Antetypes;

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
}
