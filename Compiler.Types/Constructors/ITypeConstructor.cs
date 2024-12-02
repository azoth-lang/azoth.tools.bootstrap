using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// An antetype as it is declared.
/// </summary>
/// <remarks>For generic types, a declared type is not a type, but rather a template or kind for
/// creating types.</remarks>
[Closed(
    typeof(OrdinaryTypeConstructor),
    typeof(AnyTypeConstructor),
    typeof(SimpleTypeConstructor))]
public interface ITypeConstructor : IEquatable<ITypeConstructor>
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

    IdentifierName? ContainingPackage { get; }
    NamespaceName? ContainingNamespace { get; }

    /// <summary>
    /// Whether this type can be constructed. Abstract types and type variables cannot be constructed.
    /// </summary>
    bool CanBeInstantiated { get; }

    TypeSemantics Semantics { get; }

    TypeName Name { get; }

    IFixedList<TypeConstructorParameter> Parameters { get; }

    bool AllowsVariance { get; }

    IFixedList<GenericParameterPlainType> GenericParameterPlainTypes { get; }

    IFixedSet<NamedPlainType> Supertypes { get; }

    IAntetype Construct(IEnumerable<IAntetype> typeArguments);

    IAntetype ConstructWithGenericParameterPlainTypes() => Construct(GenericParameterPlainTypes);

    IMaybeAntetype Construct(IEnumerable<IMaybeAntetype> typeArguments)
    {
        var properTypeArguments = typeArguments.ToFixedList().As<IAntetype>();
        if (properTypeArguments is null) return IAntetype.Unknown;
        return Construct(properTypeArguments.AsEnumerable());
    }

    IAntetype? TryConstructNullary();

    string ToString();
}
