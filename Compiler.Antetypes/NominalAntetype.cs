using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

/// <summary>
/// An antetype that is defined by its name.
/// </summary>
[Closed(typeof(NonGenericNominalAntetype), typeof(UserGenericNominalAntetype))]
public abstract class NominalAntetype : IAntetype
{
    public abstract IDeclaredAntetype Declared { get; }

    private protected NominalAntetype() { }

    #region Equality
    public abstract bool Equals(IMaybeExpressionAntetype? other);

    public sealed override bool Equals(object? obj)
        => obj is IMaybeExpressionAntetype other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
