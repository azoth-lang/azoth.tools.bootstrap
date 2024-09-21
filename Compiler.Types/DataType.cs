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
public abstract class DataType : Pseudotype, IMaybeExpressionType
{
    public virtual bool AllowsVariance => false;

    public virtual bool HasIndependentTypeArguments => false;

    private protected DataType() { }

    public sealed override IMaybeExpressionType ToUpperBound() => this;

    /// <summary>
    /// Convert types for constant values to their corresponding types.
    /// </summary>
    public virtual IMaybeType ToNonConstValueType() => (IMaybeType)this;

    /// <summary>
    /// The same type except with any mutability removed.
    /// </summary>
    public virtual DataType WithoutWrite() => this;
    IMaybeExpressionType IMaybeExpressionType.WithoutWrite() => WithoutWrite();

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
    IMaybeExpressionType IMaybeExpressionType.AccessedVia(Pseudotype contextType) => AccessedVia(contextType);

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public virtual DataType AccessedVia(ICapabilityConstraint capability) => this;
    IMaybeExpressionType IMaybeExpressionType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);

    #region Equality
    public abstract bool Equals(IMaybeExpressionType? other);

    public abstract override int GetHashCode();

    public override bool Equals(IMaybePseudotype? other)
        => ReferenceEquals(this, other) || other is IMaybeExpressionType dataType && Equals(dataType);

    public sealed override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((IMaybeExpressionType)obj);
    }
    #endregion
}
