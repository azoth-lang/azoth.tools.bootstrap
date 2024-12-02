using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OptionalPlainType),
    typeof(FunctionPlainType),
    typeof(OrdinaryNamedPlainType),
    typeof(NeverPlainType),
    typeof(SimpleTypeConstructor),
    typeof(VariablePlainType))]
public interface INonVoidPlainType : IPlainType, IMaybeNonVoidPlainType
{
    TypeSemantics? Semantics { get; }
}
