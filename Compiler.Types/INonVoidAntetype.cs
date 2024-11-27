using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(
    typeof(OptionalAntetype),
    typeof(FunctionAntetype),
    typeof(UserNonGenericNominalAntetype),
    typeof(UserGenericNominalAntetype),
    typeof(AnyAntetype),
    typeof(NeverAntetype),
    typeof(SimpleAntetype),
    typeof(GenericParameterAntetype),
    typeof(SelfAntetype))]
public interface INonVoidAntetype : IAntetype, IMaybeNonVoidAntetype
{
    // TODO this seems like the wrong way to do this and was introduced only for the legacy reference equality operator
    bool HasReferenceSemantics { get; }
}
