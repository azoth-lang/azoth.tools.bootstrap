using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class UserGenericNominalAntetype : NominalAntetype, INonVoidAntetype
{
    public override IDeclaredAntetype Declared { get; }
    public IFixedList<IAntetype> TypeArguments { get; }

    public UserGenericNominalAntetype(IDeclaredAntetype declaredAnteType, IEnumerable<IAntetype> typeArguments)
    {
        Declared = declaredAnteType;
        TypeArguments = typeArguments.ToFixedList();
        if (Declared.GenericParameters.Count != TypeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match. Given `[{string.Join(", ", TypeArguments)}]` for `{declaredAnteType}`.",
                nameof(typeArguments));
    }
}
