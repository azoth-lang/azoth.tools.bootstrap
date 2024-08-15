using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(
    typeof(INonVoidAntetype),
    typeof(UnknownAntetype))]
public interface IMaybeNonVoidAntetype : IMaybeAntetype
{
}
