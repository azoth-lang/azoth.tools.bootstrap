using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(
    typeof(OptionalAntetype),
    typeof(FunctionAntetype),
    typeof(UserNonGenericNominalAntetype),
    typeof(AnyAntetype),
    typeof(NeverAntetype),
    typeof(UserGenericNominalAntetype),
    typeof(SimpleAntetype))]
public interface INonVoidAntetype : IAntetype
{
}
