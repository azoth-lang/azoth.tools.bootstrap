using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

/// <summary>
/// An antetype that is defined by its name.
/// </summary>
[Closed(typeof(NonGenericNominalAntetype), typeof(UserGenericNominalAntetype))]
public abstract class NominalAntetype : IAntetype
{
    public abstract IDeclaredAntetype DeclaredAntetype { get; }
    public abstract bool AllowsVariance { get; }
    public abstract TypeName Name { get; }
    public virtual IFixedList<IAntetype> TypeArguments => FixedList.Empty<IAntetype>();
    public abstract IFixedSet<NominalAntetype> Supertypes { get; }

    private protected NominalAntetype() { }

    public abstract IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype);

    #region Equality
    public abstract bool Equals(IMaybeExpressionAntetype? other);

    public sealed override bool Equals(object? obj)
        => obj is IMaybeExpressionAntetype other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
