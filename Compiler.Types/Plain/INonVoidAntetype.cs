using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OptionalAntetype),
    typeof(FunctionAntetype),
    typeof(UserNonGenericNominalAntetype),
    typeof(NamedPlainType),
    typeof(AnyAntetype),
    typeof(NeverAntetype),
    typeof(SimpleTypeConstructor),
    typeof(GenericParameterPlainType),
    typeof(SelfAntetype))]
public interface INonVoidAntetype : IAntetype, IMaybeNonVoidAntetype
{
    // TODO this seems like the wrong way to do this and was introduced only for the legacy reference equality operator
    bool HasReferenceSemantics { get; }
}
