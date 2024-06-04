using Compiler.Antetypes.Declared;

namespace Compiler.Antetypes;

/// <summary>
/// An antetype that is not generic.
/// </summary>
/// <remarks>Non-generic antetypes are both an antetype and their own declared antetype.</remarks>
public abstract class NonGenericNominalAntetype : NominalAntetype, IDeclaredAntetype
{
    public IAntetype With(IEnumerable<IAntetype> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Non-generic type cannot have type arguments", nameof(typeArguments));
        return this;
    }
}
