using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

/// <summary>
/// The void type behaves similar to a unit type. However, it represents the
/// lack of a value. For example, a function returning `void` doesn't return
/// a value. A parameter of type `void` is dropped from the parameter list.
/// </summary>
public sealed class VoidType : EmptyType
{
    #region Singleton
    internal static readonly VoidType Instance = new VoidType();

    private VoidType()
        : base(SpecialTypeName.Void)
    { }
    #endregion

    public override VoidAntetype ToAntetype() => IAntetype.Void;
}
