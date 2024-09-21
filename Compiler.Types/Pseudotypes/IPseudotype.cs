using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

[Closed(typeof(IExpressionType), typeof(CapabilityTypeConstraint))]
public interface IPseudotype : IMaybePseudotype
{
}
