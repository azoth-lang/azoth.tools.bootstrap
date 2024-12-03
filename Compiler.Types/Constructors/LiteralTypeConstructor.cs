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
public abstract class LiteralTypeConstructor : ISimpleOrLiteralTypeConstructor
{
    public IdentifierName? ContainingPackage => null;

    public NamespaceName? ContainingNamespace => null;

    public bool CanBeInstantiated => true;

    public TypeSemantics Semantics => TypeSemantics.Value;

    public SpecialTypeName Name { get; }
    TypeName ITypeConstructor.Name => Name;

    // TODO these need type parameters that are values
    IFixedList<TypeConstructorParameter> ITypeConstructor.Parameters
        => FixedList.Empty<TypeConstructorParameter>();

    bool ITypeConstructor.AllowsVariance => false;

    IFixedList<GenericParameterPlainType> ITypeConstructor.GenericParameterPlainTypes
        => FixedList.Empty<GenericParameterPlainType>();

    // TODO should this instead include the non-literal type (e.g. `int` or `bool`)?
    IFixedSet<NamedPlainType> ITypeConstructor.Supertypes => AnyTypeConstructor.Set;

    public abstract OrdinaryNamedPlainType PlainType { get; }

    private protected LiteralTypeConstructor(SpecialTypeName name)
    {
        Name = name;
    }

    /// <summary>
    /// The default non-constant type to place values of this type in.
    /// </summary>
    public abstract ITypeConstructor ToNonLiteral();

    public IPlainType Construct(IEnumerable<IPlainType> typeArguments)
        => throw new NotImplementedException("Constructing literal types requires value type parameters.");

    public IMaybePlainType Construct(IEnumerable<IMaybePlainType> typeArguments)
    {
        var properTypeArguments = typeArguments.ToFixedList().As<IPlainType>();
        if (properTypeArguments is null) return IPlainType.Unknown;
        return Construct(properTypeArguments.AsEnumerable());
    }

    /// <remarks>All literal types take a type parameter and cannot be nullary constructed.</remarks>
    public IPlainType? TryConstructNullary() => null;

    #region Equality
    public abstract bool Equals(ITypeConstructor? other);

    public sealed override bool Equals(object? obj)
        => obj is IMaybePlainType other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
