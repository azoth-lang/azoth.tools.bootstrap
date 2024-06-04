namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class UnknownAntetype : IMaybeAntetype
{
    #region Singleton
    public static readonly UnknownAntetype Instance = new();

    private UnknownAntetype() { }
    #endregion
}
