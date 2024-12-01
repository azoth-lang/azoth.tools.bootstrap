using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OptionalAntetype),
    typeof(FunctionAntetype),
    typeof(NamedPlainType),
    typeof(AnyAntetype),
    typeof(NeverAntetype),
    typeof(SimpleTypeConstructor),
    typeof(GenericParameterPlainType),
    typeof(SelfAntetype))]
public interface INonVoidAntetype : IAntetype, IMaybeNonVoidAntetype
{
    TypeSemantics Semantics { get; }
}
