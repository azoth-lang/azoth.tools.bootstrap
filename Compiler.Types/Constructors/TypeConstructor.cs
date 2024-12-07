using System.Diagnostics.CodeAnalysis;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// A type constructor for a plain type.
/// </summary>
/// <remarks>A type constructor is a sort of template, factory, or kind for creating plain types.</remarks>
[Closed(
    typeof(OrdinaryTypeConstructor),
    typeof(AnyTypeConstructor),
    typeof(SimpleOrLiteralTypeConstructor))]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Using as a trait.")]
// ReSharper disable once InconsistentNaming
public interface TypeConstructor : IEquatable<TypeConstructor>, TypeConstructorContext
{
    #region Standard Type Constructors
    public static readonly AnyTypeConstructor Any = AnyTypeConstructor.Instance;
    public static readonly BoolTypeConstructor Bool = BoolTypeConstructor.Instance;
    public static readonly BigIntegerTypeConstructor Int = BigIntegerTypeConstructor.Int;
    public static readonly BigIntegerTypeConstructor UInt = BigIntegerTypeConstructor.UInt;
    public static readonly FixedSizeIntegerTypeConstructor Int8 = FixedSizeIntegerTypeConstructor.Int8;
    public static readonly FixedSizeIntegerTypeConstructor Byte = FixedSizeIntegerTypeConstructor.Byte;
    public static readonly FixedSizeIntegerTypeConstructor Int16 = FixedSizeIntegerTypeConstructor.Int16;
    public static readonly FixedSizeIntegerTypeConstructor UInt16 = FixedSizeIntegerTypeConstructor.UInt16;
    public static readonly FixedSizeIntegerTypeConstructor Int32 = FixedSizeIntegerTypeConstructor.Int32;
    public static readonly FixedSizeIntegerTypeConstructor UInt32 = FixedSizeIntegerTypeConstructor.UInt32;
    public static readonly FixedSizeIntegerTypeConstructor Int64 = FixedSizeIntegerTypeConstructor.Int64;
    public static readonly FixedSizeIntegerTypeConstructor UInt64 = FixedSizeIntegerTypeConstructor.UInt64;
    public static readonly PointerSizedIntegerTypeConstructor Size = PointerSizedIntegerTypeConstructor.Size;
    public static readonly PointerSizedIntegerTypeConstructor Offset = PointerSizedIntegerTypeConstructor.Offset;
    public static readonly PointerSizedIntegerTypeConstructor NInt = PointerSizedIntegerTypeConstructor.NInt;
    public static readonly PointerSizedIntegerTypeConstructor NUInt = PointerSizedIntegerTypeConstructor.NUInt;
    #endregion

    #region Literal Types
    public static readonly BoolLiteralTypeConstructor True = BoolLiteralTypeConstructor.True;
    public static readonly BoolLiteralTypeConstructor False = BoolLiteralTypeConstructor.False;
    #endregion

    void TypeConstructorContext.AppendContextPrefix(StringBuilder builder)
    {
        ToString(builder);
        builder.Append('.');
    }

    TypeConstructorContext Context { get; }

    /// <summary>
    /// Whether this type can be constructed. Abstract types and type variables cannot be constructed.
    /// </summary>
    bool CanBeInstantiated { get; }

    /// <summary>
    /// Whether this type can have fields.
    /// </summary>
    /// <remarks>Even if a type cannot have fields, a subtype still could.</remarks>
    public bool CanHaveFields { get; }

    TypeSemantics Semantics { get; }

    TypeName Name { get; }

    IFixedList<TypeConstructorParameter> Parameters { get; }

    bool AllowsVariance { get; }

    IFixedList<GenericParameterPlainType> GenericParameterPlainTypes { get; }

    IFixedSet<ConstructedPlainType> Supertypes { get; }

    IPlainType Construct(IEnumerable<IPlainType> typeArguments);

    IPlainType ConstructWithGenericParameterPlainTypes()
        => Construct(GenericParameterPlainTypes);

    IMaybePlainType Construct(IEnumerable<IMaybePlainType> typeArguments)
    {
        var properTypeArguments = typeArguments.ToFixedList().As<IPlainType>();
        if (properTypeArguments is null) return IPlainType.Unknown;
        return Construct(properTypeArguments.AsEnumerable());
    }

    IPlainType? TryConstructNullary();

    TypeConstructor ToNonLiteral() => this;

    #region Equality
    bool IEquatable<TypeConstructorContext>.Equals(TypeConstructorContext? other)
        => ReferenceEquals(this, other) || other is TypeConstructor that && Equals(that);
    #endregion

    public void ToString(StringBuilder builder);
}
