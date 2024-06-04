using Compiler.Antetypes.ConstValue;

namespace Compiler.Antetypes;

/// <summary>
/// The antetype of an expression.
/// </summary>
/// <remarks>Expressions can have constant value types that cannot be used as a type argument or the
/// type of a variable.</remarks>
public interface IExpressionAntetype : IMaybeExpressionAntetype
{
    public static readonly BoolConstValueAntetype True = BoolConstValueAntetype.True;
    public static readonly BoolConstValueAntetype False = BoolConstValueAntetype.False;
}
