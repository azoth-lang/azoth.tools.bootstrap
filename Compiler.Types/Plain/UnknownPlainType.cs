namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class UnknownPlainType : IMaybeFunctionAntetype
{
    #region Singleton
    internal static readonly UnknownPlainType Instance = new();

    private UnknownPlainType() { }
    #endregion

    public IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        => antetype;

    #region Equality
    public bool Equals(IMaybeExpressionAntetype? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        => obj is IMaybeExpressionAntetype other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(UnknownPlainType));
    #endregion

    public override string ToString() => "⧼unknown⧽";
}
