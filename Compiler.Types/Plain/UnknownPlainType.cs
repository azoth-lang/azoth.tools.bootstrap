namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class UnknownPlainType : IMaybeFunctionPlainType
{
    #region Singleton
    internal static readonly UnknownPlainType Instance = new();

    private UnknownPlainType() { }
    #endregion

    IMaybePlainType IMaybeFunctionPlainType.Return => IPlainType.Unknown;

    public IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType)
        => plainType;

    #region Equality
    public bool Equals(IMaybePlainType? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        => obj is IMaybePlainType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(UnknownPlainType));
    #endregion

    public override string ToString() => "⧼unknown⧽";
}
