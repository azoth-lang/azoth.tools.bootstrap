using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

[Closed(typeof(IExpressionAntetype), typeof(IMaybeAntetype))]
public interface IMaybeExpressionAntetype : IEquatable<IMaybeExpressionAntetype>
{
    /// <summary>
    /// Convert types for constant values to their corresponding types.
    /// </summary>
    IMaybeAntetype ToNonConstValueType();

    string ToString();
}
