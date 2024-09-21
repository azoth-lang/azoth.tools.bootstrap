using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(IExpressionType), typeof(DataType), typeof(IMaybeType))]
public interface IMaybeExpressionType : IMaybePseudotype
{
    public sealed DataType AsType => (DataType)this;

    bool AllowsVariance { get; }

    bool HasIndependentTypeArguments { get; }

    /// <summary>
    /// Convert types for constant values to their corresponding types.
    /// </summary>
    IMaybeType ToNonConstValueType();

    /// <summary>
    /// The same type except with any mutability removed.
    /// </summary>
    IMaybeExpressionType WithoutWrite();

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
