using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

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
public abstract partial class TypeConstructor : TypeConstructorContext, IEquatable<TypeConstructor>
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
        IFixedSet<ConstructedBareType> supertypes)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract: false, isConst, TypeKind.Struct, name, genericParameters, supertypes);

    public static OrdinaryTypeConstructor CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract, isConst, TypeKind.Class, name, [], BareType.AnySet);

    public static OrdinaryTypeConstructor CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract: true, isConst, TypeKind.Trait, name, [], BareType.AnySet);

    public static OrdinaryTypeConstructor CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name,
        IFixedList<Parameter> genericParameters,
        IFixedSet<ConstructedBareType> supertypes)
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
        IFixedSet<ConstructedBareType> supertypes)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract, isConst, TypeKind.Class, name, genericParameters, supertypes);

    public static OrdinaryTypeConstructor CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        StandardName name,
        IFixedList<Parameter> genericParameters,
        IFixedSet<ConstructedBareType> supertypes)
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
            genericParameters.ToFixedList(), BareType.AnySet);

    public static OrdinaryTypeConstructor CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name,
        params Parameter[] genericParameters)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract: true, isConst, TypeKind.Trait, StandardName.Create(name, genericParameters.Length),
             genericParameters.ToFixedList(), BareType.AnySet);
    #endregion

    public sealed override void AppendContextPrefix(StringBuilder builder)
    {
        ToString(builder);
        builder.Append('.');
    }

    public abstract TypeConstructorContext Context { get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as const.
    /// </summary>
    public abstract bool IsDeclaredConst { get; }

    // TODO add bool IsDeclaredMove { get; } once that is being supported

    /// <summary>
    /// Whether this type can be instantiated. Abstract types and type variables cannot be instantiated.
    /// </summary>
    /// <remarks>Even if a type can't be instantiated, a subtype may still be. Thus, there could still
    /// be instances for this type.</remarks>
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

    /// <summary>
    /// Whether this type allows any variance in its parameters (e.g. it has `out` or `in` parameters).
    /// </summary>
    public abstract bool AllowsVariance { get; }

    /// <summary>
    /// Whether any of the parameters are independent (i.e. `ind` or `sharable ind`).
    /// </summary>
    public abstract bool HasIndependentParameters { get; }

    /// <summary>
    /// The plain types used to refer to the parameters to this type within the type definition.
    /// </summary>
    public abstract IFixedList<GenericParameterPlainType> ParameterPlainTypes { get; }

    /// <summary>
    /// The types used to refer to the parameters to this type within the type definition.
    /// </summary>
    public IFixedList<GenericParameterType> ParameterTypes
        => LazyInitializer.EnsureInitialized(ref parameterTypes,
            () => ParameterPlainTypes.Select(p => new GenericParameterType(p)).ToFixedList());
    private IFixedList<GenericParameterType>? parameterTypes;

    public abstract IFixedSet<ConstructedBareType> Supertypes { get; }

    private ConstructedPlainType? withParameterPlainTypes;
    private ConstructedBareType? withParameterTypes;

    public abstract ConstructedPlainType Construct(IFixedList<IPlainType> arguments);

    /// <summary>
    /// Construct a type using the <see cref="ParameterPlainTypes"/> to create a plain type as it
    /// would be used inside the type definition.
    /// </summary>
    // TODO will this be needed once `Self` is properly used?
    public ConstructedPlainType ConstructWithParameterPlainTypes()
        => LazyInitializer.EnsureInitialized(ref withParameterPlainTypes, () => Construct(ParameterPlainTypes));

    /// <summary>
    /// Construct a type using the <see cref="ParameterTypes"/> to create a type as it would be
    /// used inside the type definition.
    /// </summary>
    // TODO will this be needed once `Self` is properly used?
    public ConstructedBareType ConstructWithParameterTypes()
        => LazyInitializer.EnsureInitialized(ref withParameterTypes,
            () => new(ConstructWithParameterPlainTypes(), ParameterTypes));

    /// <summary>
    /// Attempt to construct a plain type from this type constructor with possibly unknown arguments.
    /// If any argument is unknown, the result is the unknown plain type.
    /// </summary>
    public IMaybePlainType Construct(IFixedList<IMaybePlainType> arguments)
    {
        var properArguments = arguments.As<IPlainType>();
        if (properArguments is null) return IPlainType.Unknown;
        return Construct(properArguments);
    }

    public ConstructedBareType Construct(IFixedList<Type> arguments)
    {
        var plainType = Construct(arguments.Select(a => a.PlainType).ToFixedList());
        return new(plainType, arguments);
    }

    /// <summary>
    /// Attempt to construct a type from this type constructor with possibly unknown arguments. If
    /// any argument is unknown, the result is the unknown type.
    /// </summary>
    public ConstructedBareType? TryConstruct(IFixedList<IMaybeType> arguments)
    {
        var properTypeArguments = arguments.As<Type>();
        if (properTypeArguments is null) return null;
        return Construct(properTypeArguments);
    }

    /// <summary>
    /// Construct this type with no type arguments.
    /// </summary>
    /// <exception cref="InvalidOperationException">This type constructor takes one or more arguments.</exception>
    public ConstructedBareType ConstructNullaryType()
    {
        if (!Parameters.IsEmpty)
            throw new InvalidOperationException($"Cannot construct nullary type for type constructor `{this}` expecting arguments.");
        var plainType = Construct(FixedList.Empty<IPlainType>());
        return new(plainType, []);
    }

    /// <summary>
    /// Try to construct a plain type with type arguments. If the type constructor takes one or more
    /// arguments, <see langword="null"/> is returned.
    /// </summary>
    public abstract IPlainType? TryConstructNullaryPlainType();

    /// <summary>
    /// If this is a non-literal type, return the default non-literal type to place values of this
    /// type in. Otherwise, returns <see langword="null"/>.
    /// </summary>
    public virtual SimpleTypeConstructor? TryToNonLiteral() => null;

    #region Equality
    public abstract bool Equals(TypeConstructor? other);

    public sealed override bool Equals(TypeConstructorContext? other)
        => ReferenceEquals(this, other) || other is TypeConstructor that && Equals(that);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder);
        return builder.ToString();
    }

    public abstract void ToString(StringBuilder builder);
}
