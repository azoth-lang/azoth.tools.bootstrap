using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(INonVoidAntetype),
    typeof(IMaybeFunctionAntetype))]
public interface IMaybeNonVoidAntetype : IMaybeAntetype
{
}