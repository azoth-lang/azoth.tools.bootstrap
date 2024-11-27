using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class FunctionAntetype : INonVoidAntetype, IMaybeFunctionAntetype
{
    public IFixedList<INonVoidAntetype> Parameters { get; }
    public IAntetype Return { get; }
    public bool HasReferenceSemantics => true;

    public FunctionAntetype(IEnumerable<INonVoidAntetype> parameters, IAntetype returnAntetype)
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
        return other is FunctionAntetype that
               && Parameters.Equals(that.Parameters)
               && Return.Equals(that.Return);
    }

    public override bool Equals(object? obj) => obj is IMaybeExpressionAntetype other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Parameters, Return);
    #endregion

    public override string ToString()
        => $"({string.Join(", ", Parameters)}) -> {Return}";
}
