using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(typeof(IAntetype), typeof(UnknownAntetype))]
public interface IMaybeAntetype : IMaybeExpressionAntetype
{
}
