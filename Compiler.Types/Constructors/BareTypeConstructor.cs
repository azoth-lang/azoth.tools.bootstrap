using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
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
    typeof(SimpleOrLiteralTypeConstructor),
    typeof(AssociatedTypeConstructor))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class BareTypeConstructor : BareTypeConstructorContext, IEquatable<BareTypeConstructor>, ITypeConstructor
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
        BareTypeConstructorContext context,
        bool isConst,
        OrdinaryName name,
        IFixedList<TypeConstructorParameter> genericParameters,
        IFixedSet<BareType> supertypes)
        => new(context, isAbstract: false, isConst, TypeKind.Struct, name, genericParameters, supertypes);

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
        IFixedList<TypeConstructorParameter> genericParameters,
        IFixedSet<BareType> supertypes)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract, isConst, TypeKind.Class, OrdinaryName.Create(name, genericParameters.Count),
            genericParameters, supertypes);

    public static OrdinaryTypeConstructor CreateClass(
        BareTypeConstructorContext context,
        bool isAbstract,
        bool isConst,
        OrdinaryName name,
        IFixedList<TypeConstructorParameter> genericParameters,
        IFixedSet<BareType> supertypes)
        => new(context, isAbstract, isConst, TypeKind.Class, name, genericParameters, supertypes);

    public static OrdinaryTypeConstructor CreateTrait(
        BareTypeConstructorContext context,
        bool isConst,
        OrdinaryName name,
        IFixedList<TypeConstructorParameter> genericParameters,
        IFixedSet<BareType> supertypes)
        => new(context, isAbstract: true, isConst, TypeKind.Trait, name, genericParameters, supertypes);

    public static OrdinaryTypeConstructor CreateClass(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isAbstract,
        bool isConst,
        string name,
        params TypeConstructorParameter[] genericParameters)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract, isConst, TypeKind.Class, OrdinaryName.Create(name, genericParameters.Length),
            genericParameters.ToFixedList(), BareType.AnySet);

    public static OrdinaryTypeConstructor CreateTrait(
        IdentifierName containingPackage,
        NamespaceName containingNamespace,
        bool isConst,
        string name,
        params TypeConstructorParameter[] genericParameters)
        => new(new NamespaceContext(containingPackage, containingNamespace),
            isAbstract: true, isConst, TypeKind.Trait, OrdinaryName.Create(name, genericParameters.Length),
             genericParameters.ToFixedList(), BareType.AnySet);
    #endregion

    public sealed override void AppendContextPrefix(StringBuilder builder, BarePlainType? containingType)
    {
        if (containingType is not null)
        {
            Requires.That(Equals(containingType.TypeConstructor), nameof(containingType), "Must match the context.");
            containingType.ToString(builder);
        }
        else
            ToString(builder);

        builder.Append('.');
    }

    public abstract BareTypeConstructorContext Context { get; }

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as const.
    /// </summary>
    public abstract bool IsDeclaredConst { get; }

    internal Capability DefaultCapability
        => IsDeclaredConst ? Capability.Constant : Capability.Read;

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

    /// <summary>
    /// The semantics of types created with this constructor or <see langword="null"/> if the
    /// semantics can't be determined (e.g. because this is a type variable).
    /// </summary>
    public abstract TypeSemantics? Semantics { get; }

    public abstract UnqualifiedName Name { get; }

    public bool HasParameters => !Parameters.IsEmpty;
    public abstract IFixedList<TypeConstructorParameter> Parameters { get; }

    /// <summary>
    /// Whether this type allows any variance in its parameters (e.g. it has `out` or `in` parameters).
    /// </summary>
    public abstract bool AllowsVariance { get; }

    /// <summary>
    /// Whether any of the parameters are independent (i.e. `ind` or `shareable ind`).
    /// </summary>
    public abstract bool HasIndependentParameters { get; }

    public abstract IFixedList<GenericParameterTypeConstructor> ParameterTypeFactories { get; }

    /// <summary>
    /// The plain types used to refer to the parameters to this type within the type definition.
    /// </summary>
    public IFixedList<GenericParameterPlainType> ParameterPlainTypes
        => Lazy.Initialize(ref parameterPlainTypes, ParameterTypeFactories,
            static factories => factories.Select(p => p.PlainType).ToFixedList());
    private IFixedList<GenericParameterPlainType>? parameterPlainTypes;

    /// <summary>
    /// The types used to refer to the parameters to this type within the type definition.
    /// </summary>
    public IFixedList<GenericParameterType> ParameterTypes
        => Lazy.Initialize(ref parameterTypes, ParameterTypeFactories,
            static factories => factories.Select(p => p.Type).ToFixedList());
    private IFixedList<GenericParameterType>? parameterTypes;

    public abstract IFixedSet<BareType> Supertypes { get; }

    public SelfTypeConstructor SelfTypeConstructor
        => Lazy.Initialize(ref selfTypeConstructor, this, static bareType => new(bareType));
    private SelfTypeConstructor? selfTypeConstructor;

    private BarePlainType? withParameterPlainTypes;
    private BareType? withParameterTypes;

    public abstract BarePlainType Construct(
        BarePlainType? containingType,
        IFixedList<PlainType> arguments);
    PlainType ITypeConstructor.Construct(BarePlainType? containingType, IFixedList<PlainType> arguments)
        => Construct(containingType, arguments);

    /// <summary>
    /// Construct a type using the <see cref="ParameterPlainTypes"/> to create a plain type as it
    /// would be used inside the type definition.
    /// </summary>
    // TODO will this be needed once `Self` is properly used?
    public BarePlainType ConstructWithParameterPlainTypes()
        => Lazy.Initialize(ref withParameterPlainTypes, this, ParameterPlainTypes,
            static (typeConstructor, arguments) =>
            {
                var containingType = (typeConstructor.Context as BareTypeConstructor)?.ConstructWithParameterPlainTypes();
                return typeConstructor.Construct(containingType, arguments);
            });

    /// <summary>
    /// Construct a type using the <see cref="ParameterTypes"/> to create a type as it would be
    /// used inside the type definition.
    /// </summary>
    /// <remarks>This takes the <paramref name="plainType"/> to check for a match to help enforce
    /// consistency between plain types and types.</remarks>
    public BareType ConstructWithParameterTypes(BarePlainType plainType)
    {
        Requires.That(ConstructWithParameterPlainTypes().Equals(plainType), nameof(plainType),
            "Plain type must match.");
        return ConstructWithParameterTypes();
    }

    /// <remarks>This is internal because it allows for creating a type without verifying consistency
    /// with the matching plain type.</remarks>
    // TODO are all the uses of this proper or should more require a plain type?
    internal BareType ConstructWithParameterTypes()
        => Lazy.Initialize(ref withParameterTypes, this, ParameterTypes,
            static (typeConstructor, arguments) =>
            {
                var containingType = (typeConstructor.Context as BareTypeConstructor)?.ConstructWithParameterTypes();
                return typeConstructor.Construct(containingType, arguments);
            });

    /// <summary>
    /// Attempt to construct a plain type from this type constructor with possibly unknown arguments.
    /// If any argument is unknown, the result is the unknown plain type.
    /// </summary>
    public IMaybePlainType Construct(IMaybePlainType? containingType, IFixedList<IMaybePlainType> arguments)
    {
        var properContainingType = containingType as BarePlainType;
        if (containingType is not null && properContainingType is null) return PlainType.Unknown;
        var properArguments = arguments.As<PlainType>();
        if (properArguments is null) return PlainType.Unknown;
        return Construct(properContainingType, properArguments);
    }

    public virtual BareType Construct(BareType? containingType, IFixedList<Type> arguments)
    {
        var plainType = Construct(containingType?.PlainType, arguments.Select(a => a.PlainType).ToFixedList());
        return new(plainType, containingType, arguments);
    }

    /// <summary>
    /// Attempt to construct a type from this type constructor with possibly unknown arguments. If
    /// any argument is unknown, the result is <see langword="null"/>.
    /// </summary>
    public BareType? TryConstructBareType(BareType? containingType, IFixedList<IMaybeType> arguments)
    {
        var properTypeArguments = arguments.As<Type>();
        if (properTypeArguments is null) return null;
        return Construct(containingType, properTypeArguments);
    }

    /// <summary>
    /// Construct this type with no type arguments.
    /// </summary>
    /// <exception cref="InvalidOperationException">This type constructor takes one or more arguments.</exception>
    public BareType ConstructNullaryType(BareType? containingType) => Construct(containingType, []);

    Type? ITypeConstructor.TryConstructNullaryType(BareType? containingType)
    {
        Requires.That(Equals(Context as BareTypeConstructor, containingType?.TypeConstructor), nameof(containingType),
            "Must match the context.");
        // Type constructors require a capability, not just type parameters, to construct a full type.
        return null;
    }

    /// <summary>
    /// Try to construct a plain type with type arguments. If the type constructor takes one or more
    /// arguments, <see langword="null"/> is returned.
    /// </summary>
    public abstract PlainType? TryConstructNullaryPlainType(BarePlainType? containingType);

    /// <summary>
    /// If this is a non-literal type, return the default non-literal type to place values of this
    /// type in. Otherwise, returns <see langword="null"/>.
    /// </summary>
    public virtual SimpleTypeConstructor? TryToNonLiteral() => null;

    #region Equality
    public abstract bool Equals(BareTypeConstructor? other);

    public sealed override bool Equals(BareTypeConstructorContext? other)
        => ReferenceEquals(this, other) || other is BareTypeConstructor that && Equals(that);
    #endregion

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder);
        return builder.ToString();
    }

    public abstract void ToString(StringBuilder builder);
}
