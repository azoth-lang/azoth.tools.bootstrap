using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OptionalPlainType),
    typeof(FunctionPlainType),
    typeof(OrdinaryNamedPlainType),
    typeof(AnyAntetype),
    typeof(NeverPlainType),
    typeof(SimpleTypeConstructor),
    typeof(GenericParameterPlainType),
    typeof(SelfPlainType))]
public interface INonVoidAntetype : IAntetype, IMaybeNonVoidAntetype
{
    TypeSemantics Semantics { get; }
}
