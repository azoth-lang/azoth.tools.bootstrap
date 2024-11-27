using Azoth.Tools.Bootstrap.Compiler.Types.Plain.ConstValue;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The antetype of an expression.
/// </summary>
/// <remarks>Expressions can have constant value types that cannot be used as a type argument or the
/// type of a variable.</remarks>
[Closed(typeof(IAntetype), typeof(ConstValueAntetype))]
public interface IExpressionAntetype : IMaybeExpressionAntetype
{
    public static readonly BoolConstValueAntetype True = BoolConstValueAntetype.True;
    public static readonly BoolConstValueAntetype False = BoolConstValueAntetype.False;
}
