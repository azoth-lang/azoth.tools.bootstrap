using Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(typeof(NumericAntetype), typeof(IntegerConstValueAntetype))]
public interface INumericAntetype
{
    IExpressionAntetype Antetype { get; }
}
