using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;

public sealed class UserDeclaredGenericAntetype : IDeclaredAntetype
{
    public IFixedList<AntetypeGenericParameter> GenericParameters { get; }

    public IFixedList<GenericParameterAntetype> GenericParameterAntetypes { get; }

    public UserDeclaredGenericAntetype(IEnumerable<AntetypeGenericParameter> genericParameters)
    {
        GenericParameters = genericParameters.ToFixedList();
        GenericParameterAntetypes = GenericParameters.Select(p => new GenericParameterAntetype(this, p))
                                                     .ToFixedList();
    }

    public IAntetype With(IEnumerable<IAntetype> typeArguments)
    {
        var args = typeArguments.ToFixedList();
        if (args.Count != GenericParameters.Count)
            throw new ArgumentException("Incorrect number of type arguments.");
        return new UserGenericNominalAntetype(this, args);
    }
}
