using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;

public sealed class UserDeclaredGenericAntetype : IUserDeclaredAntetype
{
    public IdentifierName ContainingPackage { get; }
    public NamespaceName ContainingNamespace { get; }
    public StandardName Name { get; }
    public IFixedList<AntetypeGenericParameter> GenericParameters { get; }
    public IFixedList<GenericParameterAntetype> GenericParameterAntetypes { get; }

    public UserDeclaredGenericAntetype(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        StandardName name,
        IEnumerable<AntetypeGenericParameter> genericParameters)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        Name = name;
        GenericParameters = genericParameters.ToFixedList();
        Requires.That(nameof(genericParameters), Name.GenericParameterCount == GenericParameters.Count,
            "Count must match name count");
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
