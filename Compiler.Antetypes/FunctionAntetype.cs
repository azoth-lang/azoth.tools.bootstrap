using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class FunctionAntetype : INonVoidAntetype
{
    public IFixedList<INonVoidAntetype> Parameters { get; }
    public IAntetype Return { get; }

    public FunctionAntetype(IEnumerable<INonVoidAntetype> parameters, IAntetype returnAntetype)
    {
        Return = returnAntetype;
        Parameters = parameters.ToFixedList();
    }

    #region Equality
    public bool Equals(IMaybeExpressionAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FunctionAntetype that
               && Parameters.ItemsEqual<IMaybeExpressionAntetype>(that.Parameters)
               && Return.Equals(that.Return);
    }

    public override bool Equals(object? obj) => obj is IMaybeExpressionAntetype other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Parameters, Return);
    #endregion
}
