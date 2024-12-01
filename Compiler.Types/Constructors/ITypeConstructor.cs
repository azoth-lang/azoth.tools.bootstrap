using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// An antetype as it is declared.
/// </summary>
/// <remarks>For generic types, a declared type is not a type, but rather a template or kind for
/// creating types.</remarks>
[Closed(
    typeof(OrdinaryTypeConstructor),
    typeof(SimpleTypeConstructor))]
public interface ITypeConstructor : IEquatable<ITypeConstructor>
{
    /// <summary>
    /// Whether this type can be constructed. Abstract types and type variables cannot be constructed.
    /// </summary>
    bool CanBeInstantiated { get; }

    TypeSemantics Semantics { get; }

    IFixedList<TypeConstructorParameter> Parameters { get; }

    bool AllowsVariance { get; }

    IFixedList<GenericParameterPlainType> GenericParameterPlainTypes { get; }

    IAntetype Construct(IEnumerable<IAntetype> typeArguments);

    IAntetype ConstructWithGenericParameterPlainTypes() => Construct(GenericParameterPlainTypes);

    IMaybeAntetype Construct(IEnumerable<IMaybeAntetype> typeArguments)
    {
        var properTypeArguments = typeArguments.ToFixedList().As<IAntetype>();
        if (properTypeArguments is null) return IAntetype.Unknown;
        return Construct(properTypeArguments.AsEnumerable());
    }

    IAntetype? TryConstructNullary();

    string ToString();
}
