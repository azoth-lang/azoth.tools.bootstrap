using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;

[Closed(typeof(UserDeclaredGenericAntetype), typeof(UserNonGenericNominalAntetype))]
public interface IUserDeclaredAntetype : IDeclaredAntetype
{
    IdentifierName ContainingPackage { get; }
    NamespaceName ContainingNamespace { get; }
    StandardName Name { get; }
    IFixedSet<NominalAntetype> Supertypes { get; }

    // TODO this seems like the wrong way to do this and was introduced only for the legacy reference equality operator
    bool HasReferenceSemantics { get; }
}
