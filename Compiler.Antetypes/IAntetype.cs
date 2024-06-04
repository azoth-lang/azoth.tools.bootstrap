namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

/// <remarks><para>An antetype is the type that exists before reference capabilities are accounted
/// for. This is distinct from a <i>bare type</i>. A bare type lacks a reference capability at the
/// top level. However, if it is generic then a reference capability can still occur on the type
/// arguments (e.g. <c>Foo[mut Bar])</c>. Meanwhile, an antetype lacks reference capabilities
/// anywhere (e.g. <c>Foo[Bar]</c>). Antetypes are the types that would exist in Azoth if the language
/// had no reference capabilities. They are so named because they are the types that come before or
/// precede the calculation of the regular types with reference capabilities.</para></remarks>
public interface IAntetype : IExpressionAntetype, IMaybeAntetype
{
    #region Standard Types
    public static readonly UnknownAntetype Unknown = UnknownAntetype.Instance;
    public static readonly VoidAntetype Void = VoidAntetype.Instance;
    public static readonly NeverAntetype Never = NeverAntetype.Instance;
    public static readonly AnyAntetype Any = AnyAntetype.Instance;
    public static readonly BoolAntetype Bool = BoolAntetype.Instance;
    public static readonly BigIntegerAntetype Int = BigIntegerAntetype.Int;
    public static readonly BigIntegerAntetype UInt = BigIntegerAntetype.UInt;
    public static readonly FixedSizeIntegerAntetype Int8 = FixedSizeIntegerAntetype.Int8;
    public static readonly FixedSizeIntegerAntetype Byte = FixedSizeIntegerAntetype.Byte;
    public static readonly FixedSizeIntegerAntetype Int16 = FixedSizeIntegerAntetype.Int16;
    public static readonly FixedSizeIntegerAntetype UInt16 = FixedSizeIntegerAntetype.UInt16;
    public static readonly FixedSizeIntegerAntetype Int32 = FixedSizeIntegerAntetype.Int32;
    public static readonly FixedSizeIntegerAntetype UInt32 = FixedSizeIntegerAntetype.UInt32;
    public static readonly FixedSizeIntegerAntetype Int64 = FixedSizeIntegerAntetype.Int64;
    public static readonly FixedSizeIntegerAntetype UInt64 = FixedSizeIntegerAntetype.UInt64;
    public static readonly PointerSizedIntegerAntetype Size = PointerSizedIntegerAntetype.Size;
    public static readonly PointerSizedIntegerAntetype Offset = PointerSizedIntegerAntetype.Offset;
    public static readonly PointerSizedIntegerAntetype NInt = PointerSizedIntegerAntetype.NInt;
    public static readonly PointerSizedIntegerAntetype NUInt = PointerSizedIntegerAntetype.NUInt;
    #endregion
}
