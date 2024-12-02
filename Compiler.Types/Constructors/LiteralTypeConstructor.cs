using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// Base class for type constructors for the types of literal values (e.g. <c>int[V]</c>,
/// <c>bool[V]</c>, etc.).
/// </summary>
[Closed(
       typeof(BoolLiteralTypeConstructor),
       typeof(IntegerLiteralTypeConstructor))]
public abstract class LiteralTypeConstructor : IAntetype, ISimpleOrConstValueAntetype
{
    public SpecialTypeName Name { get; }

    private protected LiteralTypeConstructor(SpecialTypeName name)
    {
        Name = name;
    }

    public abstract IMaybeAntetype ToNonLiteralType();

    public IMaybeAntetype ReplaceTypeParametersIn(IMaybeAntetype antetype)
        => antetype;

    #region Equality
    public abstract bool Equals(IMaybeAntetype? other);

    public sealed override bool Equals(object? obj)
        => obj is IMaybeAntetype other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
