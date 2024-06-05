namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class UnknownAntetype : IMaybeAntetype
{
    #region Singleton
    internal static readonly UnknownAntetype Instance = new();

    private UnknownAntetype() { }
    #endregion

    #region Equality
    public bool Equals(IMaybeExpressionAntetype? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        => obj is IMaybeExpressionAntetype other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(UnknownAntetype));
    #endregion
}
