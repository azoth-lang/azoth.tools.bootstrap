using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

[Closed(typeof(IExpressionType), typeof(IMaybeType))]
public interface IMaybeExpressionType : IMaybePseudotype
{
    #region Standard Types
    /// <summary>
    /// The unknown type as <see cref="IMaybeExpressionType"/>.
    /// </summary>
    /// <remarks>There are places where the compiler cannot infer the expression type. This can be
    /// used to force the compiler to use <see cref="IMaybeExpressionType"/>.</remarks>
    public static readonly IMaybeExpressionType Unknown = UnknownType.Instance;
    #endregion

    bool AllowsVariance { get; }

    bool HasIndependentTypeArguments { get; }

    /// <summary>
    /// Convert types for constant values to their corresponding types.
    /// </summary>
    IMaybeType ToNonConstValueType();

    /// <summary>
    /// Return the type for when a value of this type is accessed via a type of the given value.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    IMaybeExpressionType AccessedVia(IMaybePseudotype contextType);

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    IMaybeExpressionType AccessedVia(ICapabilityConstraint capability);
}
