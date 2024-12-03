using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <remarks><para>An plainType is the type that exists before reference capabilities are accounted
/// for. This is distinct from a <i>bare type</i>. A bare type lacks a reference capability at the
/// top level. However, if it is generic then a reference capability can still occur on the type
/// arguments (e.g. <c>Foo[mut Bar])</c>. Meanwhile, an plainType lacks reference capabilities
/// anywhere (e.g. <c>Foo[Bar]</c>). Plain types are the types that would exist in Azoth if the language
/// had no reference capabilities. They are so named because they are the types that come before or
/// precede the calculation of the regular types with reference capabilities.</para></remarks>
[Closed(typeof(INonVoidPlainType), typeof(NamedPlainType))]
public interface IPlainType : IMaybePlainType
{
    #region Standard Types
    public new static readonly UnknownPlainType Unknown = UnknownPlainType.Instance;
    public static readonly VoidPlainType Void = VoidPlainType.Instance;
    public static readonly NeverPlainType Never = NeverPlainType.Instance;
    public static readonly OrdinaryNamedPlainType Any = AnyTypeConstructor.PlainType;
    public static readonly OrdinaryNamedPlainType Bool = TypeConstructor.Bool.PlainType;
    public static readonly OptionalPlainType OptionalBool = new(Bool);
    public static readonly OrdinaryNamedPlainType Int = TypeConstructor.Int.PlainType;
    public static readonly OrdinaryNamedPlainType UInt = TypeConstructor.UInt.PlainType;
    public static readonly OrdinaryNamedPlainType Int8 = TypeConstructor.Int8.PlainType;
    public static readonly OrdinaryNamedPlainType Byte = TypeConstructor.Byte.PlainType;
    public static readonly OrdinaryNamedPlainType Int16 = TypeConstructor.Int16.PlainType;
    public static readonly OrdinaryNamedPlainType UInt16 = TypeConstructor.UInt16.PlainType;
    public static readonly OrdinaryNamedPlainType Int32 = TypeConstructor.Int32.PlainType;
    public static readonly OrdinaryNamedPlainType UInt32 = TypeConstructor.UInt32.PlainType;
    public static readonly OrdinaryNamedPlainType Int64 = TypeConstructor.Int64.PlainType;
    public static readonly OrdinaryNamedPlainType UInt64 = TypeConstructor.UInt64.PlainType;
    public static readonly OrdinaryNamedPlainType Size = TypeConstructor.Size.PlainType;
    public static readonly OrdinaryNamedPlainType Offset = TypeConstructor.Offset.PlainType;
    public static readonly OrdinaryNamedPlainType NInt = TypeConstructor.NInt.PlainType;
    public static readonly OrdinaryNamedPlainType NUInt = TypeConstructor.NUInt.PlainType;
    #endregion

    #region Literal Types
    /// <summary>
    /// The value `none` has this type, which is `never?`.
    /// </summary>
    public static readonly OptionalPlainType None = new(Never);

    public static readonly OrdinaryNamedPlainType True = TypeConstructor.True.PlainType;
    public static readonly OrdinaryNamedPlainType False = TypeConstructor.False.PlainType;
    #endregion

    new IPlainType ToNonLiteral() => this;
    IMaybePlainType IMaybePlainType.ToNonLiteral() => ToNonLiteral();
}