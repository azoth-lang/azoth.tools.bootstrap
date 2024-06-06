using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public class AnyAntetype : INonVoidAntetype, IDeclaredAntetype
{
    #region Singleton
    internal static readonly AnyAntetype Instance = new();

    private AnyAntetype()
    {
    }
    #endregion

    public SpecialTypeName Name => SpecialTypeName.Any;

    IFixedList<AntetypeGenericParameter> IDeclaredAntetype.GenericParameters
        => FixedList.Empty<AntetypeGenericParameter>();

    IFixedList<GenericParameterAntetype> IDeclaredAntetype.GenericParameterAntetypes
        => FixedList.Empty<GenericParameterAntetype>();

    public IAntetype With(IEnumerable<IAntetype> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Any type cannot have type arguments", nameof(typeArguments));
        return this;
    }

    public IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        => antetype;

    #region Equality
    public bool Equals(IMaybeExpressionAntetype? other)
        // AnyAntetype is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public bool Equals(IDeclaredAntetype? other)
        // AnyAntetype is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        => obj is IMaybeExpressionAntetype other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(AnyAntetype));
    #endregion

    public override string ToString() => Name.ToString();
}
