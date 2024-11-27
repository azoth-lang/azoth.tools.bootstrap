using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(
    typeof(INonVoidAntetype),
    typeof(IMaybeFunctionAntetype))]
public interface IMaybeNonVoidAntetype : IMaybeAntetype
{
}
