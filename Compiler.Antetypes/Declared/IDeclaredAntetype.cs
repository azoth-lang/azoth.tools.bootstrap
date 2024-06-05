using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;

/// <summary>
/// An antetype as it is declared.
/// </summary>
/// <remarks>For generic types, a declared type is not a type, but rather a template or kind for
/// creating types.</remarks>
[Closed(
    typeof(AnyAntetype),
    typeof(NonGenericNominalAntetype),
    typeof(IUserDeclaredAntetype),
    typeof(SimpleAntetype))]
public interface IDeclaredAntetype : IEquatable<IDeclaredAntetype>
{
    IFixedList<AntetypeGenericParameter> GenericParameters { get; }

    IFixedList<GenericParameterAntetype> GenericParameterAntetypes { get; }

    IAntetype With(IEnumerable<IAntetype> typeArguments);
}
