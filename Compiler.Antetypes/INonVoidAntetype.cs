using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(
    typeof(OptionalAntetype),
    typeof(FunctionAntetype),
    typeof(UserNonGenericNominalAntetype),
    typeof(AnyAntetype),
    typeof(NeverAntetype),
    typeof(UserGenericNominalAntetype),
    typeof(SimpleAntetype),
    typeof(GenericParameterAntetype))]
public interface INonVoidAntetype : IAntetype, IMaybeNonVoidAntetype
{
    // TODO this seems like the wrong way to do this and was introduced only for the legacy reference equality operator
    bool HasReferenceSemantics { get; }
}
