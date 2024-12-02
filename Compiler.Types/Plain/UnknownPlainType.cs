namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class UnknownPlainType : IMaybeFunctionAntetype
{
    #region Singleton
    internal static readonly UnknownPlainType Instance = new();

    private UnknownPlainType() { }
    #endregion

    public IMaybeAntetype ReplaceTypeParametersIn(IMaybeAntetype antetype)
        => antetype;

    #region Equality
    public bool Equals(IMaybeAntetype? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        => obj is IMaybeAntetype other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(UnknownPlainType));
    #endregion

    public override string ToString() => "⧼unknown⧽";
}
