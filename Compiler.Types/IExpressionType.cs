using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The type of an Azoth expression. This can be either a regular type or a constant value type.
/// </summary>
[Closed(typeof(IType), typeof(ConstValueType))]
public interface IExpressionType : IMaybeExpressionType, IPseudotype
{
    /// <summary>
    /// Return the type for when a value of this type is accessed via a reference with the given capability.
    /// </summary>
    /// <remarks>This can restrict the ability to write to the value.</remarks>
    new IExpressionType AccessedVia(ICapabilityConstraint capability);
    IMaybeExpressionType IMaybeExpressionType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);
}
