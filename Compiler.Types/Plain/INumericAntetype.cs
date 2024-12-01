using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain.ConstValue;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(NumericTypeConstructor), typeof(IntegerConstValueAntetype))]
public interface INumericAntetype : IExpressionAntetype
{
    IExpressionAntetype Antetype { get; }
}
