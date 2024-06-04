using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class UserNonGenericNominalAntetype : NonGenericNominalAntetype, INonVoidAntetype, IUserDeclaredAntetype
{
    public IdentifierName ContainingPackage { get; }
    public NamespaceName ContainingNamespace { get; }
    public IdentifierName Name { get; }
    StandardName IUserDeclaredAntetype.Name => Name;

    public UserNonGenericNominalAntetype(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        IdentifierName name)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        Name = name;
    }
}
