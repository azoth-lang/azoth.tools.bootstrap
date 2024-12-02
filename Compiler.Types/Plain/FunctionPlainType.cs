using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class FunctionPlainType : INonVoidAntetype, IMaybeFunctionAntetype
{
    public TypeSemantics Semantics => TypeSemantics.Reference;
    public IFixedList<INonVoidAntetype> Parameters { get; }
    public IAntetype Return { get; }

    public FunctionPlainType(IEnumerable<INonVoidAntetype> parameters, IAntetype returnAntetype)
    {
        Return = returnAntetype;
        Parameters = parameters.ToFixedList();
    }

    public IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        => antetype;

    #region Equality
    public bool Equals(IMaybeExpressionAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionPlainType that
               && Parameters.Equals(that.Parameters)
               && Return.Equals(that.Return);
    }

    public override bool Equals(object? obj) => obj is IMaybeExpressionAntetype other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Parameters, Return);
    #endregion

    public override string ToString()
        => $"({string.Join(", ", Parameters)}) -> {Return}";
}
