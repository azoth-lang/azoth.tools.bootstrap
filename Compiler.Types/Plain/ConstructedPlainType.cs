using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// A plain type constructed from a type constructor.
/// </summary>
/// <remarks>This includes all types defined in source code, all simple types, and the `Any` type.</remarks>
public sealed class ConstructedPlainType : ConstructedOrAssociatedPlainType
{
    public override TypeConstructor TypeConstructor { get; }
    // TODO Nested Types: add ContainingType and enforce that it must match the context of the TypeConstructor
    public override TypeSemantics? Semantics => TypeConstructor.Semantics;
    public override TypeName Name => TypeConstructor.Name;
    public override bool AllowsVariance => TypeConstructor.AllowsVariance;
    public override IFixedList<PlainType> Arguments { get; }
    public override IFixedSet<ConstructedPlainType> Supertypes { get; }
    internal override PlainTypeReplacements TypeReplacements { get; }

    public ConstructedPlainType(TypeConstructor typeConstructor, IEnumerable<PlainType> typeArguments)
    {
        TypeConstructor = typeConstructor;
        Arguments = typeArguments.ToFixedList();
        if (TypeConstructor.Parameters.Count != Arguments.Count)
            throw new ArgumentException(
                $"Number of type arguments must match the type constructor. "
                + $"Given `[{string.Join(", ", Arguments)}]` for `{typeConstructor}`.",
                nameof(typeArguments));

        TypeReplacements = new(TypeConstructor, Arguments);

        Supertypes = typeConstructor.Supertypes.Select(s => ReplaceTypeParametersIn(s.PlainType)).ToFixedSet();
    }

    public override ConstructedPlainType? TryToNonLiteral()
    {
        var newTypeConstructor = TypeConstructor.TryToNonLiteral();
        if (newTypeConstructor is null) return null;
        // Literal type constructors will have parameters, whereas their corresponding non-literal
        // types won't. Thus, do not pass any type arguments.
        return new ConstructedPlainType(newTypeConstructor, []);
    }

    public override IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType)
        => TypeReplacements.ReplaceTypeParametersIn(plainType);

    public ConstructedPlainType ReplaceTypeParametersIn(ConstructedPlainType plainType)
        => TypeReplacements.ReplaceTypeParametersIn(plainType);

    #region Equality
    public override bool Equals(IMaybePlainType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is ConstructedPlainType that
               && TypeConstructor.Equals(that.TypeConstructor)
               && Arguments.Equals(that.Arguments);
    }

    public override int GetHashCode() => HashCode.Combine(TypeConstructor, Arguments);
    #endregion

    public override string ToBareString()
    {
        var builder = new StringBuilder();
        TypeConstructor.Context.AppendContextPrefix(builder);
        builder.Append(TypeConstructor.Name.ToBareString());
        return builder.ToString();
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder);
        return builder.ToString();
    }

    public void ToString(StringBuilder builder)
    {
        TypeConstructor.Context.AppendContextPrefix(builder);
        builder.Append(TypeConstructor.Name.ToBareString());
        if (Arguments.IsEmpty)
            return;
        builder.Append('[');
        builder.AppendJoin(", ", Arguments);
        builder.Append(']');
    }
}
