using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// The type constructor for the `bool` plain type.
/// </summary>
public sealed class BoolTypeConstructor : SimpleTypeConstructor
{
    #region Singleton
    internal static readonly BoolTypeConstructor Instance = new();

    private BoolTypeConstructor()
        : base(BuiltInTypeName.Bool)
    {
    }
    #endregion
}
