using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The two empty types are `never` and `void`. They are the only types with
/// no values.
/// </summary>
[Closed(
    typeof(VoidType),
    typeof(NeverType))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class EmptyType : IType
{
    public SpecialTypeName Name { get; }

    public bool IsFullyKnown => true;
    public bool AllowsVariance => false;
    public bool HasIndependentTypeArguments => false;

    private protected EmptyType(SpecialTypeName name)
    {
        Name = name;
    }

    public abstract EmptyAntetype ToAntetype();
    IMaybeAntetype IMaybeType.ToAntetype() => ToAntetype();

    public IMaybeExpressionType ToUpperBound() => this;

    /// <summary>
    /// Convert types for constant values to their corresponding types.
    /// </summary>
    public IMaybeType ToNonConstValueType() => this;

    /// <summary>
    /// The same type except with any mutability removed.
    /// </summary>
    public IMaybeExpressionType WithoutWrite() => this;

    /// <summary>
    /// Return the type for when a value of this type is accessed via a type of the given value.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public IMaybeExpressionType AccessedVia(IMaybePseudotype contextType) => this;

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public IType AccessedVia(ICapabilityConstraint capability) => this;

    #region Eqauality
    public bool Equals(IMaybePseudotype? other)
        // Empty types are all fixed instances, so a reference equality suffices
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(Name);

    public sealed override bool Equals(object? obj)
        // Empty types are all fixed instances, so a reference equality suffices
        => ReferenceEquals(this, obj);
    #endregion

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => Name.ToString();

    public string ToILString() => ToSourceCodeString();
}
