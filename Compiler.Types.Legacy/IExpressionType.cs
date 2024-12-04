using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

/// <summary>
/// The type of an Azoth expression. This can be either a regular type or a constant value type.
/// </summary>
[Closed(typeof(IType), typeof(ConstValueType))]
public interface IExpressionType : IMaybeExpressionType, IPseudotype
{
    /// <summary>
    /// Convert types for constant values to their corresponding types.
    /// </summary>
    new IType ToNonConstValueType();
    IMaybeType IMaybeExpressionType.ToNonConstValueType() => ToNonConstValueType();

    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    new IExpressionType AccessedVia(ICapabilityConstraint capability);
    IMaybeExpressionType IMaybeExpressionType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);
}
