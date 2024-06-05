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

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is UserNonGenericNominalAntetype that
               && ContainingPackage.Equals(that.ContainingPackage)
               && ContainingNamespace.Equals(that.ContainingNamespace)
               && Name.Equals(that.Name);
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingPackage, ContainingNamespace, Name);
    #endregion
}
