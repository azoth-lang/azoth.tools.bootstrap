using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <remarks><para>An plainType is the type that exists before reference capabilities are accounted
/// for. This is distinct from a <i>bare type</i>. A bare type lacks a reference capability at the
/// top level. However, if it is generic then a reference capability can still occur on the type
/// arguments (e.g. <c>Foo[mut Bar])</c>. Meanwhile, an plainType lacks reference capabilities
/// anywhere (e.g. <c>Foo[Bar]</c>). Antetypes are the types that would exist in Azoth if the language
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
    public static readonly OrdinaryNamedPlainType Bool = ITypeConstructor.Bool.PlainType;
    public static readonly OptionalPlainType OptionalBool = new(Bool);
    public static readonly OrdinaryNamedPlainType Int = ITypeConstructor.Int.PlainType;
    public static readonly OrdinaryNamedPlainType UInt = ITypeConstructor.UInt.PlainType;
    public static readonly OrdinaryNamedPlainType Int8 = ITypeConstructor.Int8.PlainType;
    public static readonly OrdinaryNamedPlainType Byte = ITypeConstructor.Byte.PlainType;
    public static readonly OrdinaryNamedPlainType Int16 = ITypeConstructor.Int16.PlainType;
    public static readonly OrdinaryNamedPlainType UInt16 = ITypeConstructor.UInt16.PlainType;
    public static readonly OrdinaryNamedPlainType Int32 = ITypeConstructor.Int32.PlainType;
    public static readonly OrdinaryNamedPlainType UInt32 = ITypeConstructor.UInt32.PlainType;
    public static readonly OrdinaryNamedPlainType Int64 = ITypeConstructor.Int64.PlainType;
    public static readonly OrdinaryNamedPlainType UInt64 = ITypeConstructor.UInt64.PlainType;
    public static readonly OrdinaryNamedPlainType Size = ITypeConstructor.Size.PlainType;
    public static readonly OrdinaryNamedPlainType Offset = ITypeConstructor.Offset.PlainType;
    public static readonly OrdinaryNamedPlainType NInt = ITypeConstructor.NInt.PlainType;
    public static readonly OrdinaryNamedPlainType NUInt = ITypeConstructor.NUInt.PlainType;
    #endregion

    #region Literal Types
    /// <summary>
    /// The value `none` has this type, which is `never?`.
    /// </summary>
    public static readonly OptionalPlainType None = new(Never);

    public static readonly OrdinaryNamedPlainType True = ITypeConstructor.True.PlainType;
    public static readonly OrdinaryNamedPlainType False = ITypeConstructor.False.PlainType;
    #endregion
}
