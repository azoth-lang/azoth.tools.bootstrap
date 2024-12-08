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
/// <remarks><para>A type constructor is a sort of template, factory, or kind for creating plain
/// types.</para>
///
/// <para>Note that this is an interface so that classes can use explicit interface implementation
/// to hide members (e.g. <see cref="AnyTypeConstructor"/> hides <see cref="Context"/>.).</para></remarks>
[Closed(
    typeof(OrdinaryTypeConstructor),
    typeof(AnyTypeConstructor),
    typeof(SimpleOrLiteralTypeConstructor))]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Using as a trait.")]
// ReSharper disable once InconsistentNaming
public partial interface TypeConstructor : IEquatable<TypeConstructor>, TypeConstructorContext
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

    #region CreateX(...)
    public static OrdinaryTypeConstructor CreateStruct(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        StandardName name,
        IFixedList<Parameter> genericParameters,
        IFixedSet<Supertype> supertypes)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract: false, isConst, TypeKind.Struct, name, genericParameters, supertypes);

    public static OrdinaryTypeConstructor CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract, isConst, TypeKind.Class, name, [], Supertype.AnySet);

    public static OrdinaryTypeConstructor CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract: true, isConst, TypeKind.Trait, name, [], Supertype.AnySet);

    public static OrdinaryTypeConstructor CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name,
        IFixedList<Parameter> genericParameters,
        IFixedSet<Supertype> supertypes)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract, isConst, TypeKind.Class, StandardName.Create(name, genericParameters.Count),
            genericParameters, supertypes);

    public static OrdinaryTypeConstructor CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        StandardName name,
        IFixedList<Parameter> genericParameters,
        IFixedSet<Supertype> supertypes)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract, isConst, TypeKind.Class, name, genericParameters, supertypes);

    public static OrdinaryTypeConstructor CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        StandardName name,
        IFixedList<Parameter> genericParameters,
        IFixedSet<Supertype> supertypes)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract: true, isConst, TypeKind.Trait, name, genericParameters, supertypes);

    public static OrdinaryTypeConstructor CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name,
        params Parameter[] genericParameters)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract, isConst, TypeKind.Class, StandardName.Create(name, genericParameters.Length),
            genericParameters.ToFixedList(), Supertype.AnySet);

    public static OrdinaryTypeConstructor CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name,
        params Parameter[] genericParameters)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract: true, isConst, TypeKind.Trait, StandardName.Create(name, genericParameters.Length),
             genericParameters.ToFixedList(), Supertype.AnySet);
    #endregion

    void TypeConstructorContext.AppendContextPrefix(StringBuilder builder)
    {
        ToString(builder);
        builder.Append('.');
    }

    TypeConstructorContext Context { get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as const.
    /// </summary>
    public bool IsDeclaredConst { get; }

    /// <summary>
    /// Whether this type can be constructed. Abstract types and type variables cannot be constructed.
    /// </summary>
    bool CanBeInstantiated { get; }

    /// <summary>
    /// Whether this type can have fields.
    /// </summary>
    /// <remarks>Even if a type cannot have fields, a subtype still could.</remarks>
    public bool CanHaveFields { get; }

    /// <summary>
    /// Whether this type is allowed to be used as a supertype.
    /// </summary>
    public bool CanBeSupertype { get; }

    TypeSemantics Semantics { get; }
    TypeName Name { get; }

    bool HasParameters { get; }
    IFixedList<Parameter> Parameters { get; }

    bool AllowsVariance { get; }

    bool HasIndependentParameters { get; }

    IFixedList<GenericParameterPlainType> ParameterPlainTypes { get; }

    IFixedSet<Supertype> Supertypes { get; }

    ConstructedPlainType Construct(IFixedList<IPlainType> typeArguments);

    ConstructedPlainType ConstructWithParameterPlainTypes()
        => Construct(ParameterPlainTypes);

    IMaybePlainType Construct(IFixedList<IMaybePlainType> typeArguments)
    {
        var properTypeArguments = typeArguments.As<IPlainType>();
        if (properTypeArguments is null) return IPlainType.Unknown;
        return Construct(properTypeArguments);
    }

    IPlainType? TryConstructNullary();

    TypeConstructor ToNonLiteral() => this;

    #region Equality
    bool IEquatable<TypeConstructorContext>.Equals(TypeConstructorContext? other)
        => ReferenceEquals(this, other) || other is TypeConstructor that && Equals(that);
    #endregion

    public void ToString(StringBuilder builder);
}
