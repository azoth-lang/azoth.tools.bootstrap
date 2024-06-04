using Compiler.Antetypes.Declared;

namespace Compiler.Antetypes;

public class AnyAntetype : INonVoidAntetype, IDeclaredAntetype
{
    #region Singleton
    internal static readonly AnyAntetype Instance = new();

    private AnyAntetype()
    {
    }
    #endregion

    public IAntetype With(IEnumerable<IAntetype> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Any type cannot have type arguments", nameof(typeArguments));
        return this;
    }
}
