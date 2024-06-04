namespace Compiler.Antetypes.Declared;

public sealed class UserDeclaredGenericAntetype : IDeclaredAntetype
{
    public IAntetype With(IEnumerable<IAntetype> typeArguments) => throw new NotImplementedException();
}
