using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class UserGenericNominalAntetype : NominalAntetype, INonVoidAntetype
{
    public override IDeclaredAntetype Declared { get; }
    public IFixedList<IAntetype> TypeArguments { get; }

    public UserGenericNominalAntetype(IDeclaredAntetype declared, IEnumerable<IAntetype> typeArguments)
    {
        Declared = declared;
        TypeArguments = typeArguments.ToFixedList();
    }
}
