namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class OptionalPlainType : INonVoidAntetype
{
    /// <remarks>The optional type acts as a value type that may contain a reference and as such
    /// always has value semantics.</remarks>
    public TypeSemantics? Semantics => TypeSemantics.Value;
    public INonVoidAntetype Referent { get; }

    public OptionalPlainType(INonVoidAntetype referent)
    {
        Referent = referent;
    }

    public IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        => antetype;

    #region Equality
    public bool Equals(IMaybeExpressionAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OptionalPlainType that
            && Referent.Equals(that.Referent);
    }

    public override bool Equals(object? obj)
        => obj is IMaybeExpressionAntetype other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Referent);
    #endregion

    public override string ToString() => $"{Referent}?";
}
