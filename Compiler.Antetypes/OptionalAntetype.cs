namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class OptionalAntetype : INonVoidAntetype
{
    public INonVoidAntetype Referent { get; }

    public OptionalAntetype(INonVoidAntetype referent)
    {
        Referent = referent;
    }

    #region Equality
    public bool Equals(IMaybeExpressionAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OptionalAntetype that
            && Referent.Equals(that.Referent);
    }

    public override bool Equals(object? obj)
        => obj is IMaybeExpressionAntetype other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Referent);
    #endregion
}
