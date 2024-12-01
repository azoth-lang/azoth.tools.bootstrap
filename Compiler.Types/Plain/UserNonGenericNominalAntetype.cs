using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class UserNonGenericNominalAntetype : NonGenericNominalAntetype, IOrdinaryTypeConstructor, INonVoidAntetype
{
    public IdentifierName ContainingPackage { get; }
    public NamespaceName ContainingNamespace { get; }
    public bool IsAbstract { get; }
    public override bool CanBeInstantiated => !IsAbstract;
    public override IdentifierName Name { get; }
    StandardName IOrdinaryTypeConstructor.Name => Name;
    public override IFixedSet<NominalAntetype> Supertypes { get; }
    private readonly AntetypeReplacements antetypeReplacements;
    public bool HasReferenceSemantics { get; }

    public UserNonGenericNominalAntetype(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        IdentifierName name,
        IFixedSet<NominalAntetype> supertypes,
        bool hasReferenceSemantics)
    {
        ContainingPackage = containingPackage;
        ContainingNamespace = containingNamespace;
        Name = name;
        Supertypes = supertypes;
        HasReferenceSemantics = hasReferenceSemantics;
        IsAbstract = isAbstract;
        antetypeReplacements = new(this, TypeArguments);
    }

    public override IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        // A non-generic antetype can have replacements if it inherits from a generic antetype.
        => antetypeReplacements.ReplaceTypeParametersIn(antetype);

    public override NominalAntetype With(IEnumerable<IAntetype> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Non-generic type cannot have type arguments", nameof(typeArguments));
        return this;
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
