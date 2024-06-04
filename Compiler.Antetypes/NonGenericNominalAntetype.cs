using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

/// <summary>
/// An antetype that is not generic.
/// </summary>
/// <remarks>Non-generic antetypes are both an antetype and their own declared antetype.</remarks>
[Closed(typeof(EmptyAntetype), typeof(GenericParameterAntetype), typeof(UserNonGenericNominalAntetype))]
public abstract class NonGenericNominalAntetype : NominalAntetype, IDeclaredAntetype
{
    public override IDeclaredAntetype Declared => this;

    IFixedList<AntetypeGenericParameter> IDeclaredAntetype.GenericParameters
        => FixedList.Empty<AntetypeGenericParameter>();

    IFixedList<GenericParameterAntetype> IDeclaredAntetype.GenericParameterAntetypes
        => FixedList.Empty<GenericParameterAntetype>();

    public IAntetype With(IEnumerable<IAntetype> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Non-generic type cannot have type arguments", nameof(typeArguments));
        return this;
    }
}
