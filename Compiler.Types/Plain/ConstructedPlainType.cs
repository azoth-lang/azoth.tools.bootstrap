using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// A plain type constructed from a type constructor.
/// </summary>
/// <remarks>This includes all types defined in source code, all simple types, and the `Any` type.</remarks>
public sealed class ConstructedPlainType : ConstructedOrVariablePlainType
{
    public override TypeConstructor TypeConstructor { get; }
    // TODO Nested Types: add ContainingType and enforce that it must match the context of the TypeConstructor
    public override TypeSemantics? Semantics => TypeConstructor.Semantics;
    public override TypeName Name => TypeConstructor.Name;
    public override bool AllowsVariance => TypeConstructor.AllowsVariance;
    public override IFixedList<IPlainType> TypeArguments { get; }
    public override IFixedSet<ConstructedPlainType> Supertypes { get; }
    private readonly PlainTypeReplacements plainTypeReplacements;

    public ConstructedPlainType(TypeConstructor typeConstructor, IEnumerable<IPlainType> typeArguments)
    {
        TypeConstructor = typeConstructor;
        TypeArguments = typeArguments.ToFixedList();
        if (TypeConstructor.Parameters.Count != TypeArguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match the type constructor. "
                + $"Given `[{string.Join(", ", TypeArguments)}]` for `{typeConstructor}`.",
                nameof(typeArguments));

        plainTypeReplacements = new(TypeConstructor, TypeArguments);

        Supertypes = typeConstructor.Supertypes.Select(s => (ConstructedPlainType)ReplaceTypeParametersIn(s)).ToFixedSet();
    }

    public override IPlainType ToNonLiteral()
    {
        var newTypeConstructor = TypeConstructor.ToNonLiteral();
        // Avoid constructing a new object when not needed.
        if (TypeConstructor.Equals(newTypeConstructor)) return this;
        // Literal type constructors will have parameters, whereas their corresponding non-literal
        // types won't. Thus, do not pass any type arguments.
        return new ConstructedPlainType(newTypeConstructor, []);
    }

    public override IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType)
        => plainTypeReplacements.ReplaceTypeParametersIn(plainType);

    #region Equality
    public override bool Equals(IMaybePlainType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ConstructedPlainType that
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
        builder.Append(TypeConstructor.Context);
        builder.Append(TypeConstructor.Name.ToBareString());
        if (TypeArguments.IsEmpty)
            return;
        builder.Append('[');
        builder.AppendJoin(", ", TypeArguments);
        builder.Append(']');
    }
}
