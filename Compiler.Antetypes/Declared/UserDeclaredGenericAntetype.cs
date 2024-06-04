using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;

public sealed class UserDeclaredGenericAntetype : IDeclaredAntetype
{
    public IAntetype With(IEnumerable<IAntetype> typeArguments)
    {
        var args = typeArguments.ToFixedList();
        // TODO if args length doesn't match throw invalid argument exception
        return new UserGenericNominalAntetype(this, args);
    }
}
