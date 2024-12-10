using System.Diagnostics;
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
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract partial class TypeConstructor : IEquatable<TypeConstructor>, TypeConstructorContext
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

    public abstract TypeConstructorContext Context { get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as const.
    /// </summary>
    public abstract bool IsDeclaredConst { get; }

    /// <summary>
    /// Whether this type can be constructed. Abstract types and type variables cannot be constructed.
    /// </summary>
    public abstract bool CanBeInstantiated { get; }

    /// <summary>
    /// Whether this type can have fields.
    /// </summary>
    /// <remarks>Even if a type cannot have fields, a subtype still could.</remarks>
    public abstract bool CanHaveFields { get; }

    /// <summary>
    /// Whether this type is allowed to be used as a supertype.
    /// </summary>
    public abstract bool CanBeSupertype { get; }

    public abstract TypeSemantics Semantics { get; }
    public abstract TypeName Name { get; }

    public bool HasParameters => !Parameters.IsEmpty;
    public abstract IFixedList<Parameter> Parameters { get; }

    public abstract bool AllowsVariance { get; }

    public abstract bool HasIndependentParameters { get; }

    public abstract IFixedList<GenericParameterPlainType> ParameterPlainTypes { get; }

    public abstract IFixedSet<Supertype> Supertypes { get; }

    public abstract ConstructedPlainType Construct(IFixedList<IPlainType> typeArguments);

    public ConstructedPlainType ConstructWithParameterPlainTypes()
        => Construct(ParameterPlainTypes);

    public IMaybePlainType Construct(IFixedList<IMaybePlainType> typeArguments)
    {
        var properTypeArguments = typeArguments.As<IPlainType>();
        if (properTypeArguments is null) return IPlainType.Unknown;
        return Construct(properTypeArguments);
    }

    public abstract IPlainType? TryConstructNullary();

    /// <summary>
    /// The default non-constant type to place values of this type in.
    /// </summary>
    public virtual TypeConstructor ToNonLiteral() => this;

    #region Equality
    public abstract bool Equals(TypeConstructor? other);

    public sealed override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is TypeConstructor other && Equals(other);

    bool IEquatable<TypeConstructorContext>.Equals(TypeConstructorContext? other)
        => ReferenceEquals(this, other) || other is TypeConstructor that && Equals(that);

    public abstract override int GetHashCode();
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder);
        return builder.ToString();
    }

    public abstract void ToString(StringBuilder builder);
}
