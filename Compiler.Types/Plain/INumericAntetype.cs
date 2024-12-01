using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(NumericTypeConstructor), typeof(IntegerLiteralTypeConstructor))]
public interface INumericAntetype : IExpressionAntetype
{
    IExpressionAntetype Antetype { get; }
}
