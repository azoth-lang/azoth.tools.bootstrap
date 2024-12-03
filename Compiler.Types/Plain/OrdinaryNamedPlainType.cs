using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class OrdinaryNamedPlainType : NamedPlainType, INonVoidPlainType
{
    public override ITypeConstructor TypeConstructor { get; }
    public NamespaceName? ContainingNamespace => TypeConstructor.ContainingNamespace;
    public TypeSemantics? Semantics => TypeConstructor.Semantics;
    public override TypeName Name => TypeConstructor.Name;
    public override bool AllowsVariance => TypeConstructor.AllowsVariance;
    public override IFixedList<IPlainType> TypeArguments { get; }
    public override IFixedSet<NamedPlainType> Supertypes { get; }
    private readonly PlainTypeReplacements plainTypeReplacements;

    public OrdinaryNamedPlainType(ITypeConstructor typeConstructor, IEnumerable<IPlainType> typeArguments)
    {
        TypeConstructor = typeConstructor;
        TypeArguments = typeArguments.ToFixedList();
        if (TypeConstructor.Parameters.Count != TypeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match the type constructor. "
                + $"Given `[{string.Join(", ", TypeArguments)}]` for `{typeConstructor}`.",
                nameof(typeArguments));

        plainTypeReplacements = new(TypeConstructor, TypeArguments);

        Supertypes = typeConstructor.Supertypes.Select(s => (NamedPlainType)ReplaceTypeParametersIn(s)).ToFixedSet();
    }

    public IMaybePlainType ToNonLiteral()
    {
        var newTypeConstructor = TypeConstructor.ToNonLiteral();
        if (TypeConstructor.Equals(newTypeConstructor)) return this;
        return new OrdinaryNamedPlainType(newTypeConstructor, TypeArguments);
    }

    public override IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType)
        => plainTypeReplacements.ReplaceTypeParametersIn(plainType);

    #region Equality
    public override bool Equals(IMaybePlainType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OrdinaryNamedPlainType that
               && TypeConstructor.Equals(that.TypeConstructor)
               && TypeArguments.Equals(that.TypeArguments);
    }

    public override int GetHashCode() => HashCode.Combine(TypeConstructor, TypeArguments);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder);
        return builder.ToString();
    }

    public void ToString(StringBuilder builder)
    {
        var containingNamespace = ContainingNamespace;
        if (containingNamespace is not null)
        {
            builder.Append(containingNamespace);
            if (containingNamespace != NamespaceName.Global) builder.Append('.');
        }
        builder.Append(TypeConstructor.Name.ToBareString());
        if (TypeArguments.IsEmpty)
            return;
        builder.Append('[');
        builder.AppendJoin(", ", TypeArguments);
        builder.Append(']');
    }
}
