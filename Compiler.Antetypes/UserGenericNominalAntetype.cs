using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

public sealed class UserGenericNominalAntetype : NominalAntetype, INonVoidAntetype
{
    public override UserDeclaredGenericAntetype DeclaredAntetype { get; }
    public override bool AllowsVariance => DeclaredAntetype.AllowsVariance;
    public override StandardName Name => DeclaredAntetype.Name;
    public bool HasReferenceSemantics => DeclaredAntetype.HasReferenceSemantics;
    public override IFixedList<IAntetype> TypeArguments { get; }
    public override IFixedSet<NominalAntetype> Supertypes { get; }
    private readonly AntetypeReplacements antetypeReplacements;

    public UserGenericNominalAntetype(UserDeclaredGenericAntetype declaredAnteType, IEnumerable<IAntetype> typeArguments)
    {
        DeclaredAntetype = declaredAnteType;
        TypeArguments = typeArguments.ToFixedList();
        if (DeclaredAntetype.GenericParameters.Count != TypeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match. Given `[{string.Join(", ", TypeArguments)}]` for `{declaredAnteType}`.",
                nameof(typeArguments));

        antetypeReplacements = new(DeclaredAntetype, TypeArguments);

        Supertypes = declaredAnteType.Supertypes.Select(s => (NominalAntetype)ReplaceTypeParametersIn(s)).ToFixedSet();
    }

    public override IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        => antetypeReplacements.ReplaceTypeParametersIn(antetype);

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is UserGenericNominalAntetype that
               && DeclaredAntetype.Equals(that.DeclaredAntetype)
               && TypeArguments.Equals(that.TypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(DeclaredAntetype, TypeArguments);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder);
        return builder.ToString();
    }

    public void ToString(StringBuilder builder)
    {
        builder.Append(DeclaredAntetype.ContainingNamespace);
        if (DeclaredAntetype.ContainingNamespace != NamespaceName.Global) builder.Append('.');
        builder.Append(DeclaredAntetype.Name.ToBareString());
        builder.Append('[');
        builder.AppendJoin(", ", TypeArguments);
        builder.Append(']');
    }
}
