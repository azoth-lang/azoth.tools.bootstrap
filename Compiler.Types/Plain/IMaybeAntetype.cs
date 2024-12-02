using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(IAntetype), typeof(IMaybeNonVoidAntetype))]
public interface IMaybeAntetype : IMaybeExpressionAntetype
{
    #region Standard Types
    /// <summary>
    /// The unknown type as <see cref="IMaybeAntetype"/>.
    /// </summary>
    /// <remarks>There are places where the compiler cannot infer the expression type. This can be
    /// used to force the compiler to use <see cref="IMaybeAntetype"/>.</remarks>
    public static readonly IMaybeAntetype Unknown = UnknownPlainType.Instance;
    #endregion

    IMaybeAntetype IMaybeExpressionAntetype.ToNonLiteralType() => this;
}
