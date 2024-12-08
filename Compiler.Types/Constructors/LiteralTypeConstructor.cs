using System.Text;
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
// TODO possibly remove this type and make the literal types simple types
public abstract class LiteralTypeConstructor : SimpleOrLiteralTypeConstructor
{
    /// <remarks>Literal types don't carry state, so they don't have fields.</remarks>
    public override bool CanHaveFields => false;

    public sealed override bool CanBeInstantiated => true;

    public sealed override TypeSemantics Semantics => TypeSemantics.Value;

    public sealed override SpecialTypeName Name { get; }

    // TODO these need type parameters that are values
    public sealed override IFixedList<TypeConstructor.Parameter> Parameters => [];

    public sealed override bool AllowsVariance => false;

    public override bool HasIndependentParameters => false;

    public sealed override IFixedList<GenericParameterPlainType> ParameterPlainTypes => [];

    // TODO should this instead include the non-literal type (e.g. `int` or `bool`)?
    public sealed override IFixedSet<TypeConstructor.Supertype> Supertypes => TypeConstructor.Supertype.AnySet;

    public abstract ConstructedPlainType PlainType { get; }

    private protected LiteralTypeConstructor(SpecialTypeName name)
    {
        Name = name;
    }

    /// <summary>
    /// The default non-constant type to place values of this type in.
    /// </summary>
    public abstract TypeConstructor ToNonLiteral();

    public sealed override ConstructedPlainType Construct(IFixedList<IPlainType> typeArguments)
       => throw new NotImplementedException("Constructing literal types requires value type parameters.");

    public IMaybePlainType Construct(IFixedList<IMaybePlainType> typeArguments)
    {
        var properTypeArguments = typeArguments.As<IPlainType>();
        if (properTypeArguments is null) return IPlainType.Unknown;
        return Construct(properTypeArguments);
    }

    /// <remarks>All literal types take a type parameter and cannot be nullary constructed.</remarks>
    public sealed override IPlainType? TryConstructNullary() => null;

    #region Equality
    public abstract override bool Equals(TypeConstructor? other);

    public sealed override bool Equals(object? obj)
        => obj is IMaybePlainType other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public override void ToString(StringBuilder builder) => builder.Append(ToString());
}
