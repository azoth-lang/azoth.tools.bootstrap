namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class UnknownAntetype : IMaybeAntetype
{
    #region Singleton
    internal static readonly UnknownAntetype Instance = new();

    private UnknownAntetype() { }
    #endregion
}
