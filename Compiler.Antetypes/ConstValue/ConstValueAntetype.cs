using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;

[Closed(
       typeof(BoolConstValueAntetype),
       typeof(IntegerConstValueAntetype))]
public abstract class ConstValueAntetype : IExpressionAntetype, ISimpleOrConstValueAntetype
{
    public SpecialTypeName Name { get; }

    private protected ConstValueAntetype(SpecialTypeName name)
    {
        Name = name;
    }

    public abstract IMaybeAntetype ToNonConstValueType();

    public IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        => antetype;

    #region Equality
    public abstract bool Equals(IMaybeExpressionAntetype? other);

    public sealed override bool Equals(object? obj)
        => obj is IMaybeExpressionAntetype other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
