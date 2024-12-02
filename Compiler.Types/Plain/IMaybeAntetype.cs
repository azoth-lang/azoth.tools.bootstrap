using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(IAntetype), typeof(IMaybeNonVoidAntetype))]
public interface IMaybeAntetype : IEquatable<IMaybeAntetype>
{
    #region Standard Types
    /// <summary>
    /// The unknown type as <see cref="IMaybeAntetype"/>.
    /// </summary>
    /// <remarks>There are places where the compiler cannot infer the expression type. This can be
    /// used to force the compiler to use <see cref="IMaybeAntetype"/>.</remarks>
    public static readonly IMaybeAntetype Unknown = UnknownPlainType.Instance;
    #endregion

    /// <summary>
    /// Convert types for literals (e.g. <c>bool[true]</c>, <c>int[42]</c> etc.) to their
    /// corresponding types.
    /// </summary>
    IMaybeAntetype ToNonLiteralType() => this;

    IMaybeAntetype ReplaceTypeParametersIn(IMaybeAntetype antetype);

    string ToString();
}
