using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The antetype of an expression.
/// </summary>
/// <remarks>Expressions can have constant value types that cannot be used as a type argument or the
/// type of a variable.</remarks>
[Closed(typeof(IAntetype), typeof(LiteralTypeConstructor))]
public interface IExpressionAntetype : IMaybeExpressionAntetype;
