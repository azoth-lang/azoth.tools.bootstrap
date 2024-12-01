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
    typeof(AnyAntetype),
    typeof(NonGenericNominalAntetype),
    typeof(IOrdinaryTypeConstructor),
    typeof(SimpleTypeConstructor))]
public interface ITypeConstructor : IEquatable<ITypeConstructor>
{
    /// <summary>
    /// Whether this type can be constructed. Abstract types and type variables cannot be constructed.
    /// </summary>
    bool CanBeConstructed { get; }

    IFixedList<AntetypeGenericParameter> GenericParameters { get; }

    bool AllowsVariance { get; }

    IFixedList<GenericParameterAntetype> GenericParameterAntetypes { get; }

    IAntetype With(IEnumerable<IAntetype> typeArguments);

    IAntetype WithGenericParameterAntetypes() => With(GenericParameterAntetypes);

    IMaybeAntetype With(IEnumerable<IMaybeAntetype> typeArguments)
    {
        var properTypeArguments = typeArguments.ToFixedList().As<IAntetype>();
        if (properTypeArguments is null) return IAntetype.Unknown;
        return With(properTypeArguments.AsEnumerable());
    }

    string ToString();
}
