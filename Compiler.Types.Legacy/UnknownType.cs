using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

/// <summary>
/// The type of expressions and values whose type could not be determined or
/// was somehow invalid. The unknown type can't be directly used in code.
/// No well typed program contains any expression with an unknown type.
/// </summary>
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class UnknownType : IMaybeFunctionType, IMaybeParameterType, IMaybeSelfParameterType
{
    #region Singleton
    internal static readonly UnknownType Instance = new();

    private UnknownType() { }
    #endregion

    #region Parmaeter Types
    bool IMaybeSelfParameterType.IsLent => false;
    bool IMaybeParameterType.IsLent => false;

    IMaybePseudotype IMaybeSelfParameterType.Type => this;
    IMaybeNonVoidType IMaybeParameterType.Type => this;
    #endregion

    public bool AllowsVariance => false;
    public bool HasIndependentTypeArguments => false;

    IMaybeType IMaybeFunctionType.Return => IType.Unknown;

    IMaybeNonVoidType IMaybeNonVoidType.WithoutWrite() => this;

    public IMaybeAntetype ToAntetype() => IAntetype.Unknown;

    public IMaybeExpressionType ToUpperBound() => this;

    /// <summary>
    /// Convert types for constant values to their corresponding types.
    /// </summary>
    public IMaybeType ToNonConstValueType() => this;

    /// <summary>
    /// The same type except with any mutability removed.
    /// </summary>
    public IMaybeType WithoutWrite() => this;

    /// <summary>
    /// Return the type for when a value of this type is accessed via a type of the given value.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public IMaybeType AccessedVia(IMaybePseudotype contextType)
    {
        if (contextType is CapabilityType capabilityType) return AccessedVia(capabilityType.Capability);
        if (contextType is CapabilityTypeConstraint capabilityTypeConstraint)
            return AccessedVia(capabilityTypeConstraint.Capability);
        return this;
    }

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    public IMaybeType AccessedVia(ICapabilityConstraint capability) => this;

    public bool Equals(IMaybePseudotype? other)
        // The unknown type is a singleton, so reference equality suffices
        => ReferenceEquals(this, other);

    #region Equals
    public bool Equals(IMaybeExpressionType? other)
        // The unknown type is a singleton, so reference equality suffices
        => ReferenceEquals(this, other);

    public sealed override bool Equals(object? obj)
        // The unknown type is a singleton, so reference equality suffices
        => ReferenceEquals(this, obj);

    public override int GetHashCode() => HashCode.Combine(typeof(UnknownType));
    #endregion

    public override string ToString() => throw new NotSupportedException();

    /// <remarks><see cref="ToSourceCodeString"/> is used to format error messages. As such, it
    /// is necessary to provide some output for the unknown type in case it appears in an error
    /// message.</remarks>
    public string ToSourceCodeString() => "⧼unknown⧽";

    public string ToILString() => "⧼unknown⧽";
}
