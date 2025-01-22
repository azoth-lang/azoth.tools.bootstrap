using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// A plain type for a type that can be a bare type.
/// </summary>
/// <remarks>This includes all types defined in source code, all simple types, and the `Any` type.</remarks>
public sealed class BarePlainType : NonVoidPlainType
{
    public BareTypeConstructor TypeConstructor { [DebuggerStepThrough] get; }
    public BarePlainType? ContainingType { [DebuggerStepThrough] get; }
    public override TypeSemantics? Semantics => TypeConstructor.Semantics;
    public UnqualifiedName Name => TypeConstructor.Name;
    public bool AllowsVariance => TypeConstructor.AllowsVariance;
    public IFixedList<PlainType> Arguments { [DebuggerStepThrough] get; }
    public IFixedSet<BarePlainType> Supertypes
        => Lazy.Initialize(ref supertypes, TypeConstructor, TypeReplacements,
            static (typeConstructor, replacements)
                => typeConstructor.Supertypes.Select(s => replacements.Apply(s.PlainType)).ToFixedSet());
    private IFixedSet<BarePlainType>? supertypes;
    public override PlainTypeReplacements TypeReplacements
        => Lazy.Initialize(ref typeReplacements, this, static plainType => new(plainType));
    private PlainTypeReplacements? typeReplacements;

    public BarePlainType(
        BareTypeConstructor typeConstructor,
        BarePlainType? containingType,
        IEnumerable<PlainType> typeArguments)
    {
        Requires.That(Equals(typeConstructor.Context as BareTypeConstructor, containingType?.TypeConstructor),
            nameof(containingType),
            "Does not match type constructor.");
        TypeConstructor = typeConstructor;
        ContainingType = containingType;
        Arguments = typeArguments.ToFixedList();
        Requires.That(TypeConstructor.Parameters.Count == Arguments.Count, nameof(typeArguments),
            $"Number of type arguments must match the type constructor. "
            + $"Given `[{string.Join(", ", Arguments)}]` for `{typeConstructor}`.");
    }

    public override BarePlainType? TryToNonLiteral()
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
        return other is BarePlainType that
               && TypeConstructor.Equals(that.TypeConstructor)
               && Arguments.Equals(that.Arguments);
    }

    public override int GetHashCode() => HashCode.Combine(TypeConstructor, Arguments);
    #endregion

    public string ToBareString()
    {
        var builder = new StringBuilder();
        TypeConstructor.Context.AppendContextPrefix(builder, ContainingType);
        // TODO remove special case for literal types once they properly have arguments
        if (TypeConstructor is LiteralTypeConstructor)
            builder.Append(TypeConstructor);
        else
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
        // TODO remove special case for literal types once they properly have arguments
        if (TypeConstructor is LiteralTypeConstructor)
            builder.Append(TypeConstructor);
        else
            builder.Append(TypeConstructor.Name.ToBareString());
        if (!Arguments.IsEmpty)
        {
            builder.Append('[');
            builder.AppendJoin(", ", Arguments);
            builder.Append(']');
        }
    }
}
