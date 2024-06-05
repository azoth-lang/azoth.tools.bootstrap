using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;

public abstract class ConstValueAntetype : IExpressionAntetype
{
    public SpecialTypeName Name { get; }

    private protected ConstValueAntetype(SpecialTypeName name)
    {
        Name = name;
    }

    public abstract IMaybeAntetype ToNonConstValueType();

    #region Equality
    public abstract bool Equals(IMaybeExpressionAntetype? other);

    public sealed override bool Equals(object? obj)
        => obj is IMaybeExpressionAntetype other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
