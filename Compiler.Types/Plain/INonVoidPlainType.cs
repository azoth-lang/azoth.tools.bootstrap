using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OptionalPlainType),
    typeof(FunctionPlainType),
    typeof(ConstructedPlainType),
    typeof(NeverPlainType),
    typeof(VariablePlainType))]
public interface INonVoidPlainType : IPlainType, IMaybeNonVoidPlainType
{
    TypeSemantics? Semantics { get; }
}
