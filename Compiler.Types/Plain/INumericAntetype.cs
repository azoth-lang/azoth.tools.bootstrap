using Azoth.Tools.Bootstrap.Compiler.Types.Plain.ConstValue;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(NumericAntetype), typeof(IntegerConstValueAntetype))]
public interface INumericAntetype : IExpressionAntetype
{
    IExpressionAntetype Antetype { get; }
}
