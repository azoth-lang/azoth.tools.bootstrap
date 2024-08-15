using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(typeof(IAntetype), typeof(IMaybeNonVoidAntetype))]
public interface IMaybeAntetype : IMaybeExpressionAntetype
{
    IMaybeAntetype IMaybeExpressionAntetype.ToNonConstValueType() => this;
}
