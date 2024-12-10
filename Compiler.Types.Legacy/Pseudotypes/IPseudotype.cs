using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;

[Closed(typeof(IType), typeof(CapabilityTypeConstraint))]
public interface IPseudotype : IMaybePseudotype
{
    new IType ToUpperBound();
    IMaybeType IMaybePseudotype.ToUpperBound() => ToUpperBound();
    Decorated.IType ToDecoratedType();
}
