using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// An antetype that is defined by its name.
/// </summary>
// TODO but shouldn't simple antetypes be nominal by this definition?
[Closed(typeof(NonGenericNominalAntetype), typeof(OrdinaryNamedPlainType))]
public abstract class NominalAntetype : IAntetype
{
    public abstract ITypeConstructor TypeConstructor { get; }
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
