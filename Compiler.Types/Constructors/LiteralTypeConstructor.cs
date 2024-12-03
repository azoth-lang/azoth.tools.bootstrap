using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// Base class for type constructors for the types of literal values (e.g. <c>int[V]</c>,
/// <c>bool[V]</c>, etc.).
/// </summary>
[Closed(
       typeof(BoolLiteralTypeConstructor),
       typeof(IntegerLiteralTypeConstructor))]
public abstract class LiteralTypeConstructor : SimpleOrLiteralTypeConstructor
{
    public sealed override IdentifierName? ContainingPackage => null;

    public sealed override NamespaceName? ContainingNamespace => null;

    public sealed override bool CanBeInstantiated => true;

    public sealed override TypeSemantics Semantics => TypeSemantics.Value;

    public sealed override SpecialTypeName Name { get; }

    // TODO these need type parameters that are values
    public sealed override IFixedList<TypeConstructorParameter> Parameters
        => FixedList.Empty<TypeConstructorParameter>();

    public sealed override bool AllowsVariance => false;

    public sealed override IFixedList<GenericParameterPlainType> GenericParameterPlainTypes
        => FixedList.Empty<GenericParameterPlainType>();

    // TODO should this instead include the non-literal type (e.g. `int` or `bool`)?
    public sealed override IFixedSet<NamedPlainType> Supertypes => AnyTypeConstructor.Set;

    public abstract OrdinaryNamedPlainType PlainType { get; }

    private protected LiteralTypeConstructor(SpecialTypeName name)
    {
        Name = name;
    }

    /// <summary>
    /// The default non-constant type to place values of this type in.
    /// </summary>
    public abstract TypeConstructor ToNonLiteral();

    public sealed override IPlainType Construct(IEnumerable<IPlainType> typeArguments)
       => throw new NotImplementedException("Constructing literal types requires value type parameters.");

    public IMaybePlainType Construct(IEnumerable<IMaybePlainType> typeArguments)
    {
        var properTypeArguments = typeArguments.ToFixedList().As<IPlainType>();
        if (properTypeArguments is null) return IPlainType.Unknown;
        return Construct(properTypeArguments.AsEnumerable());
    }

    /// <remarks>All literal types take a type parameter and cannot be nullary constructed.</remarks>
    public sealed override IPlainType? TryConstructNullary() => null;

    #region Equality
    public abstract override bool Equals(TypeConstructor? other);

    public sealed override bool Equals(object? obj)
        => obj is IMaybePlainType other && Equals(other);

    public abstract override int GetHashCode();
    #endregion
}
