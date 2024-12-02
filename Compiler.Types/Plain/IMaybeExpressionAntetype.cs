using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(IExpressionAntetype), typeof(IMaybeAntetype))]
public interface IMaybeExpressionAntetype : IEquatable<IMaybeExpressionAntetype>
{
    /// <summary>
    /// Convert types for literals (e.g. <c>bool[true]</c>, <c>int[42]</c> etc.) to their
    /// corresponding types.
    /// </summary>
    IMaybeAntetype ToNonLiteralType();

    IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype);

    string ToString();
}
