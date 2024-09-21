using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
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
public abstract class DataType : IMaybeExpressionType
{
    public virtual bool AllowsVariance => false;

    public virtual bool HasIndependentTypeArguments => false;

    /// <summary>
    /// A known type is one that has no unknown parts.
    /// </summary>
    public abstract bool IsFullyKnown { get; }

    private protected DataType() { }

    public IMaybeExpressionType ToUpperBound() => this;

    /// <summary>
    /// Convert this type to the equivalent antetype.
    /// </summary>
    public abstract IMaybeExpressionAntetype ToAntetype();

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
    public DataType AccessedVia(IMaybePseudotype contextType)
    {
        if (contextType is CapabilityType capabilityType)
            return AccessedVia(capabilityType.Capability);
        if (contextType is CapabilityTypeConstraint capabilityTypeConstraint)
            return AccessedVia(capabilityTypeConstraint.Capability);
        return this;
    }
    IMaybeExpressionType IMaybeExpressionType.AccessedVia(IMaybePseudotype contextType) => AccessedVia(contextType);

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public virtual DataType AccessedVia(ICapabilityConstraint capability) => this;
    IMaybeExpressionType IMaybeExpressionType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);

    #region Equality
    public abstract bool Equals(IMaybeExpressionType? other);

    public abstract override int GetHashCode();

    public bool Equals(IMaybePseudotype? other)
        => ReferenceEquals(this, other) || other is IMaybeExpressionType dataType && Equals(dataType);

    public sealed override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((IMaybeExpressionType)obj);
    }
    #endregion

    /// <summary>
    /// How this type would be written in source code.
    /// </summary>
    public abstract string ToSourceCodeString();

    /// <summary>
    /// How this type would be written in IL.
    /// </summary>
    public abstract string ToILString();
}
