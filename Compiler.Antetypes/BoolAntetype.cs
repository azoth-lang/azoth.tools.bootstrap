using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class BoolAntetype : SimpleAntetype
{
    #region Singleton
    internal static readonly BoolAntetype Instance = new();

    private BoolAntetype()
        : base(SpecialTypeName.Bool)
    {
    }
    #endregion
}
