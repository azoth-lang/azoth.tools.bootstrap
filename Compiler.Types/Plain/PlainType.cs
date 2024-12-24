using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <remarks><para>An plainType is the type that exists before reference capabilities are accounted
/// for. This is distinct from a <i>bare type</i>. A bare type lacks a reference capability at the
/// top level. However, if it is generic then a reference capability can still occur on the type
/// arguments (e.g. <c>Foo[mut Bar])</c>. Meanwhile, an plainType lacks reference capabilities
/// anywhere (e.g. <c>Foo[Bar]</c>). Plain types are the types that would exist in Azoth if the language
/// had no reference capabilities. They are so named because they are the types that come before or
/// precede the calculation of the regular types with reference capabilities.</para></remarks>
[Closed(
    typeof(NonVoidPlainType),
    typeof(VoidPlainType))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class PlainType : IMaybePlainType
{
    internal static readonly IFixedSet<ConstructedPlainType> AnySet
        = AnyTypeConstructor.PlainType.Yield().ToFixedSet();

    #region Standard Types
    public static readonly UnknownPlainType Unknown = UnknownPlainType.Instance;
    public static readonly VoidPlainType Void = VoidPlainType.Instance;
    public static readonly NeverPlainType Never = NeverPlainType.Instance;
    public static readonly ConstructedPlainType Any = AnyTypeConstructor.PlainType;
    public static readonly ConstructedPlainType Bool = BareTypeConstructor.Bool.PlainType;
    public static readonly OptionalPlainType OptionalBool = new(Bool);
    public static readonly ConstructedPlainType Int = BareTypeConstructor.Int.PlainType;
    public static readonly ConstructedPlainType UInt = BareTypeConstructor.UInt.PlainType;
    public static readonly ConstructedPlainType Int8 = BareTypeConstructor.Int8.PlainType;
    public static readonly ConstructedPlainType Byte = BareTypeConstructor.Byte.PlainType;
    public static readonly ConstructedPlainType Int16 = BareTypeConstructor.Int16.PlainType;
    public static readonly ConstructedPlainType UInt16 = BareTypeConstructor.UInt16.PlainType;
    public static readonly ConstructedPlainType Int32 = BareTypeConstructor.Int32.PlainType;
    public static readonly ConstructedPlainType UInt32 = BareTypeConstructor.UInt32.PlainType;
    public static readonly ConstructedPlainType Int64 = BareTypeConstructor.Int64.PlainType;
    public static readonly ConstructedPlainType UInt64 = BareTypeConstructor.UInt64.PlainType;
    public static readonly ConstructedPlainType Size = BareTypeConstructor.Size.PlainType;
    public static readonly ConstructedPlainType Offset = BareTypeConstructor.Offset.PlainType;
    public static readonly ConstructedPlainType NInt = BareTypeConstructor.NInt.PlainType;
    public static readonly ConstructedPlainType NUInt = BareTypeConstructor.NUInt.PlainType;
    #endregion

    #region Literal Types
    /// <summary>
    /// The value `none` has this type, which is `never?`.
    /// </summary>
    public static readonly OptionalPlainType None = new(Never);

    public static readonly ConstructedPlainType True = BareTypeConstructor.True.PlainType;
    public static readonly ConstructedPlainType False = BareTypeConstructor.False.PlainType;
    #endregion

    public virtual PlainTypeReplacements TypeReplacements => PlainTypeReplacements.None;

    public virtual ConstructedPlainType? TryToNonLiteral() => null;
    public virtual PlainType ToNonLiteral() => TryToNonLiteral() ?? this;
    IMaybePlainType IMaybePlainType.ToNonLiteral() => ToNonLiteral();

    #region Equality
    public abstract bool Equals(IMaybePlainType? other);

    public sealed override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is IMaybePlainType other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
