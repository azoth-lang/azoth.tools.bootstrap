using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OptionalPlainType),
    typeof(FunctionPlainType),
    typeof(ConstructedOrVariablePlainType),
    typeof(NeverPlainType))]
public abstract class NonVoidPlainType : IPlainType, IMaybeNonVoidPlainType
{
    public abstract TypeSemantics? Semantics { get; }

    public virtual IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType) => this;

    #region Equality
    public abstract bool Equals(IMaybePlainType? other);

    public sealed override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is IMaybePlainType other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
