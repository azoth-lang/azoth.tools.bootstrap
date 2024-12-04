using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OptionalPlainType),
    typeof(FunctionPlainType),
    typeof(ConstructedOrVariablePlainType),
    typeof(NeverPlainType))]
public interface INonVoidPlainType : IPlainType, IMaybeNonVoidPlainType
{
    TypeSemantics? Semantics { get; }
}
