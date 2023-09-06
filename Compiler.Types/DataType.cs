using System;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The data type of a value in an Azoth program. This includes potentially
/// unresolved types like `UnknownType` or types containing unknown parts.
/// </summary>
[Closed(
    typeof(ReferenceType),
    typeof(ValueType),
    typeof(EmptyType),
    typeof(UnknownType))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class DataType : IEquatable<DataType>
{
    #region Standard Types
    public static readonly UnknownType Unknown = UnknownType.Instance;
    public static readonly VoidType Void = VoidType.Instance;
    public static readonly NeverType Never = NeverType.Instance;
    public static readonly BoolType Bool = BoolType.Instance;
    public static readonly BoolConstantType True = BoolConstantType.True;
    public static readonly BoolConstantType False = BoolConstantType.False;
    public static readonly FixedSizeIntegerType Byte = FixedSizeIntegerType.Byte;
    public static readonly BigIntegerType Int = BigIntegerType.Int;
    public static readonly BigIntegerType UInt = BigIntegerType.UInt;
    public static readonly FixedSizeIntegerType Int32 = FixedSizeIntegerType.Int32;
    public static readonly FixedSizeIntegerType UInt32 = FixedSizeIntegerType.UInt32;
    public static readonly PointerSizedIntegerType Size = PointerSizedIntegerType.Size;
    public static readonly PointerSizedIntegerType Offset = PointerSizedIntegerType.Offset;

    /// <summary>
    /// The value `none` has this type, which is `never?`.
    /// </summary>
    public static readonly OptionalType None = new(Never);
    #endregion

    /// <summary>
    /// The `never` and `void` types are the only empty types. This means
    /// there are no values of either type. The `never` type is defined
    /// as the type without values. The `void` type behaves more like a unit
    /// type. However, its implementation is that it doesn't have a value
    /// and represents the lack of that value. For example, that a function
    /// doesn't return a value or that an argument is to be dropped.
    /// </summary>
    public virtual bool IsEmpty => false;

    /// <summary>
    /// Whether this is a type for constants like the integer constant type.
    /// </summary>
    public virtual bool IsConstant => false;

    /// <summary>
    /// A known type is one that has no unknown parts.
    /// </summary>
    public abstract bool IsKnown { get; }

    /// <summary>
    /// The semantics of values of this type.
    /// </summary>
    public abstract TypeSemantics Semantics { get; }

    private protected DataType() { }

    /// <summary>
    /// Convert types for literal constants to their corresponding types.
    /// </summary>
    public virtual DataType ToNonConstantType() => this;

    [Obsolete("Use ToSourceCodeString() or ToILString() instead", error: true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public sealed override string ToString()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        => throw new NotSupportedException();

    /// <summary>
    /// How this type would be written in source code.
    /// </summary>
    public abstract string ToSourceCodeString();

    /// <summary>
    /// How this type would be written in IL.
    /// </summary>
    public abstract string ToILString();

    /// <summary>
    /// Returns the same type except with any mutability removed.
    /// </summary>
    public virtual DataType WithoutWrite() => this;

    #region Equality
    public abstract bool Equals(DataType? other);
    public abstract override int GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((DataType)obj);
    }

    public static bool operator ==(DataType? left, DataType? right)
        => Equals(left, right);

    public static bool operator !=(DataType? left, DataType? right)
        => !Equals(left, right);
    #endregion
}
