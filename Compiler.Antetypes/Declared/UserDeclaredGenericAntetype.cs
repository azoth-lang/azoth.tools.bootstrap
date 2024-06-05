using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;

public sealed class UserDeclaredGenericAntetype : IUserDeclaredAntetype
{
    public IdentifierName ContainingPackage { get; }
    public NamespaceName ContainingNamespace { get; }
    public GenericName Name { get; }
    StandardName IUserDeclaredAntetype.Name => Name;
    public IFixedList<AntetypeGenericParameter> GenericParameters { get; }
    public IFixedList<GenericParameterAntetype> GenericParameterAntetypes { get; }

    public UserDeclaredGenericAntetype(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        GenericName name,
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

    #region Equality
    public bool Equals(IDeclaredAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is UserDeclaredGenericAntetype that
               && ContainingPackage.Equals(that.ContainingPackage)
               && ContainingNamespace.Equals(that.ContainingNamespace)
               && Name.Equals(that.Name)
               && GenericParameters.ItemsEqual(that.GenericParameters);
        // GenericParameterAntetypes is derived from GenericParameters and doesn't need to be compared
    }

    public override int GetHashCode()
        => HashCode.Combine(ContainingPackage, ContainingNamespace, Name, GenericParameters);
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
        if (!GenericParameters.Any()) return;

        builder.Append('[');
        builder.AppendJoin(", ", GenericParameters);
        builder.Append(']');
    }
}
