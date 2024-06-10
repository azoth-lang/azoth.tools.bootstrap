using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class UserNonGenericNominalAntetype : NonGenericNominalAntetype, INonVoidAntetype, IUserDeclaredAntetype
{
    public IdentifierName ContainingPackage { get; }
    public NamespaceName ContainingNamespace { get; }
    public override IdentifierName Name { get; }
    StandardName IUserDeclaredAntetype.Name => Name;
    private readonly Lazy<IFixedSet<NominalAntetype>> lazySupertypes;
    public override IFixedSet<NominalAntetype> Supertypes => lazySupertypes.Value;
    private readonly AntetypeReplacements antetypeReplacements;
    public bool HasReferenceSemantics { get; }

    public UserNonGenericNominalAntetype(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        IdentifierName name,
        Lazy<IFixedSet<NominalAntetype>> lazySupertypes,
        bool hasReferenceSemantics)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        Name = name;
        this.lazySupertypes = lazySupertypes;
        HasReferenceSemantics = hasReferenceSemantics;
        antetypeReplacements = new(this, TypeArguments);
    }

    public override IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        // A non-generic antetype can have replacements if it inherits from a generic antetype.
        => antetypeReplacements.ReplaceTypeParametersIn(antetype);

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

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder);
        return builder.ToString();
    }

    public void ToString(StringBuilder builder)
    {
        builder.Append(ContainingPackage);
        builder.Append("::.");
        builder.Append(ContainingNamespace);
        if (ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(Name.ToBareString());
    }
}
