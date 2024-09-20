using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
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
public abstract class DataType : Pseudotype, IEquatable<DataType>, IMaybeType
{
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
    /// Whether this type in some way allows there to be write-aliases to the reachable object graph.
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

    public override bool Equals(Pseudotype? other)
        => ReferenceEquals(this, other) || other is DataType dataType && Equals(dataType);

    public bool Equals(IMaybeType? other)
        => ReferenceEquals(this, other) || Equals((DataType?)other);

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
