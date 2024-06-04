using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The data type of a value in an Azoth program. This includes potentially
/// unresolved types like <see cref="UnknownType"/> or types containing unknown parts.
/// </summary>
[Closed(
    typeof(Type),
    typeof(UnknownType))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class DataType : Pseudotype, IEquatable<DataType>
{
    // TODO move these to `Type`
    #region Standard Types
    public static readonly DataType Unknown = UnknownType.Instance;
    public static readonly Promise<UnknownType> PromiseOfUnknown = Promise.ForValue(UnknownType.Instance);
    public static readonly VoidType Void = VoidType.Instance;
    public static readonly NeverType Never = NeverType.Instance;
    public static readonly Promise<NeverType> PromiseOfNever = Promise.ForValue(Never);
    public static readonly CapabilityType<BoolType> Bool = DeclaredType.Bool.Type;
    public static readonly BoolConstValueType True = BoolConstValueType.True;
    public static readonly BoolConstValueType False = BoolConstValueType.False;
    public static readonly CapabilityType<BigIntegerType> Int = DeclaredType.Int.Type;
    public static readonly CapabilityType<BigIntegerType> UInt = DeclaredType.UInt.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> Int8 = DeclaredType.Int8.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> Byte = DeclaredType.Byte.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> Int16 = DeclaredType.Int16.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> UInt16 = DeclaredType.UInt16.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> Int32 = DeclaredType.Int32.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> UInt32 = DeclaredType.UInt32.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> Int64 = DeclaredType.Int64.Type;
    public static readonly CapabilityType<FixedSizeIntegerType> UInt64 = DeclaredType.UInt64.Type;
    public static readonly CapabilityType<PointerSizedIntegerType> Size = DeclaredType.Size.Type;
    public static readonly CapabilityType<PointerSizedIntegerType> Offset = DeclaredType.Offset.Type;
    public static readonly CapabilityType<PointerSizedIntegerType> NInt = DeclaredType.NInt.Type;
    public static readonly CapabilityType<PointerSizedIntegerType> NUInt = DeclaredType.NUInt.Type;

    /// <summary>
    /// The value `none` has this type, which is `never?`.
    /// </summary>
    public static readonly OptionalType None = new(Never);
    public static readonly Promise<OptionalType> PromiseOfNone = Promise.ForValue(None);
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
    /// Whether this is a type for constant values like specific integer or boolean values.
    /// </summary>
    public virtual bool IsTypeOfConstValue => false;

    /// <summary>
    /// Whether this type allows for writing to instances of it.
    /// </summary>
    public virtual bool AllowsWrite => false;

    /// <summary>
    /// Whether this type in some way allows there to be write aliases to the to reachable object
    /// graph.
    /// </summary>
    public virtual bool AllowsWriteAliases => false;

    public virtual bool AllowsVariance => false;

    public virtual bool HasIndependentTypeArguments => false;

    private protected DataType() { }

    public sealed override DataType ToUpperBound() => this;

    /// <summary>
    /// Convert types for constant values to their corresponding types.
    /// </summary>
    public virtual DataType ToNonConstValueType() => this;

    /// <summary>
    /// The same type except with any mutability removed.
    /// </summary>
    public virtual DataType WithoutWrite() => this;

    /// <summary>
    /// Return the type for when a value of this type is accessed via a type of the given value.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public DataType AccessedVia(Pseudotype contextType)
    {
        if (contextType is CapabilityType capabilityType)
            return AccessedVia(capabilityType.Capability);
        if (contextType is CapabilityTypeConstraint capabilityTypeConstraint)
            return AccessedVia(capabilityTypeConstraint.Capability);
        return this;
    }

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public virtual DataType AccessedVia(ICapabilityConstraint capability) => this;

    #region Equality
    public abstract bool Equals(DataType? other);

    public abstract override int GetHashCode();

    public sealed override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((DataType)obj);
    }

    public static bool operator ==(DataType? left, DataType? right) => Equals(left, right);

    public static bool operator !=(DataType? left, DataType? right) => !Equals(left, right);
    #endregion
}
