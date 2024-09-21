using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(NonEmptyType), typeof(EmptyType))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class Type : IExpressionType
{
    public virtual bool AllowsVariance => false;
    public virtual bool HasIndependentTypeArguments => false;

    /// <summary>
    /// A known type is one that has no unknown parts.
    /// </summary>
    public abstract bool IsFullyKnown { get; }

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
    public virtual IMaybeExpressionType WithoutWrite() => this;

    /// <summary>
    /// Return the type for when a value of this type is accessed via a type of the given value.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public IMaybeExpressionType AccessedVia(IMaybePseudotype contextType)
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
    public virtual Type AccessedVia(ICapabilityConstraint capability) => this;
    IMaybeExpressionType IMaybeExpressionType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);

    #region Equality
    public abstract bool Equals(IMaybeExpressionType? other);
    public abstract override int GetHashCode();

    public bool Equals(IMaybePseudotype? other)
        => ReferenceEquals(this, other) || other is IMaybeExpressionType type && Equals(type);

    public sealed override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is IMaybeExpressionType type && Equals(type);
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
