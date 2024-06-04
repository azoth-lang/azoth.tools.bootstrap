using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Compiler.Antetypes;

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
