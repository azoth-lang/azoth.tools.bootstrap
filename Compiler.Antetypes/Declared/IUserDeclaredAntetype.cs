using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;

[Closed(typeof(UserDeclaredGenericAntetype), typeof(UserNonGenericNominalAntetype))]
public interface IUserDeclaredAntetype : IDeclaredAntetype
{
    IdentifierName ContainingPackage { get; }
    NamespaceName ContainingNamespace { get; }
    StandardName Name { get; }
}
