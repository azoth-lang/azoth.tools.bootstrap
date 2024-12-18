using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// A plain type constructed from a type constructor.
/// </summary>
/// <remarks>This includes all types defined in source code, all simple types, and the `Any` type.</remarks>
public sealed class ConstructedPlainType : NonVoidPlainType
{
    public TypeConstructor TypeConstructor { get; }
    public ConstructedPlainType? ContainingType { get; }
    public override TypeSemantics? Semantics => TypeConstructor.Semantics;
    public TypeName Name => TypeConstructor.Name;
    public bool AllowsVariance => TypeConstructor.AllowsVariance;
    public IFixedList<PlainType> Arguments { get; }
    public IFixedSet<ConstructedPlainType> Supertypes
        => Lazy.Initialize(ref supertypes, TypeConstructor, TypeReplacements,
            static (typeConstructor, replacements)
                => typeConstructor.Supertypes.Select(s => replacements.Apply(s.PlainType)).ToFixedSet());
    private IFixedSet<ConstructedPlainType>? supertypes;
    public override PlainTypeReplacements TypeReplacements
        => Lazy.Initialize(ref typeReplacements, this, static plainType => new(plainType));
    private PlainTypeReplacements? typeReplacements;

    public ConstructedPlainType(
        TypeConstructor typeConstructor,
        ConstructedPlainType? containingType,
        IEnumerable<PlainType> typeArguments)
    {
        Requires.That(Equals(typeConstructor.Context as TypeConstructor, containingType?.TypeConstructor),
            nameof(containingType),
            "Does not match type constructor.");
        TypeConstructor = typeConstructor;
        ContainingType = containingType;
        Arguments = typeArguments.ToFixedList();
        Requires.That(TypeConstructor.Parameters.Count == Arguments.Count, nameof(typeArguments),
            $"Number of type arguments must match the type constructor. "
            + $"Given `[{string.Join(", ", Arguments)}]` for `{typeConstructor}`.");
    }

    public override ConstructedPlainType? TryToNonLiteral()
    {
        // TODO handle containing type is the literal type?
        var newTypeConstructor = TypeConstructor.TryToNonLiteral();
        if (newTypeConstructor is null) return null;
        // Literal type constructors will have parameters, whereas their corresponding non-literal
        // types won't. Thus, do not pass any type arguments.
        return new(newTypeConstructor, containingType: null, []);
    }

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

    public string ToBareString()
    {
        var builder = new StringBuilder();
        TypeConstructor.Context.AppendContextPrefix(builder, ContainingType);
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
        TypeConstructor.Context.AppendContextPrefix(builder, ContainingType);
        builder.Append(TypeConstructor.Name.ToBareString());
        if (!Arguments.IsEmpty)
        {
            builder.Append('[');
            builder.AppendJoin(", ", Arguments);
            builder.Append(']');
        }
    }
}
