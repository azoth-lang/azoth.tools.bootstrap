using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OptionalPlainType),
    typeof(FunctionPlainType),
    typeof(ConstructedOrVariablePlainType),
    typeof(NeverPlainType))]
// TODO consider converting to class
public interface INonVoidPlainType : IPlainType, IMaybeNonVoidPlainType
{
    TypeSemantics? Semantics { get; }
}
