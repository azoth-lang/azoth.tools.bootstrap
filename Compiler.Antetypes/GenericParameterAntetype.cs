using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class GenericParameterAntetype : NonGenericNominalAntetype
{
    public UserDeclaredGenericAntetype DeclaringAntetype { get; }
    public AntetypeGenericParameter Parameter { get; }
    public IdentifierName Name => Parameter.Name;

    public GenericParameterAntetype(UserDeclaredGenericAntetype declaringAntetype, AntetypeGenericParameter parameter)
    {
        DeclaringAntetype = declaringAntetype;
        Parameter = parameter;
    }
}
