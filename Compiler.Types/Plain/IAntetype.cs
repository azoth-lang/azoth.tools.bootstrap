using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <remarks><para>An antetype is the type that exists before reference capabilities are accounted
/// for. This is distinct from a <i>bare type</i>. A bare type lacks a reference capability at the
/// top level. However, if it is generic then a reference capability can still occur on the type
/// arguments (e.g. <c>Foo[mut Bar])</c>. Meanwhile, an antetype lacks reference capabilities
/// anywhere (e.g. <c>Foo[Bar]</c>). Antetypes are the types that would exist in Azoth if the language
/// had no reference capabilities. They are so named because they are the types that come before or
/// precede the calculation of the regular types with reference capabilities.</para></remarks>
[Closed(typeof(INonVoidAntetype), typeof(NamedPlainType))]
public interface IAntetype : IExpressionAntetype, IMaybeAntetype
{
    #region Standard Types
    public new static readonly UnknownAntetype Unknown = UnknownAntetype.Instance;
    public static readonly VoidAntetype Void = VoidAntetype.Instance;
    public static readonly NeverAntetype Never = NeverAntetype.Instance;
    public static readonly AnyAntetype Any = AnyAntetype.Instance;
    public static readonly BoolTypeConstructor Bool = BoolTypeConstructor.Instance;
    public static readonly OptionalAntetype OptionalBool = new(Bool);
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

    /// <summary>
    /// The value `none` has this type, which is `never?`.
    /// </summary>
    public static readonly OptionalAntetype None = new(Never);
    #endregion
}
