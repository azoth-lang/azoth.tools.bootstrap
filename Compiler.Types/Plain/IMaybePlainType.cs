using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(IPlainType), typeof(IMaybeNonVoidPlainType))]
public interface IMaybePlainType : IEquatable<IMaybePlainType>
{
    #region Standard Types
    /// <summary>
    /// The unknown type as <see cref="IMaybePlainType"/>.
    /// </summary>
    /// <remarks>There are places where the compiler cannot infer the expression type. This can be
    /// used to force the compiler to use <see cref="IMaybePlainType"/>.</remarks>
    public static readonly IMaybePlainType Unknown = UnknownPlainType.Instance;
    #endregion

    /// <summary>
    /// Convert types for literals (e.g. <c>bool[true]</c>, <c>int[42]</c> etc.) to their
    /// corresponding types.
    /// </summary>
    IMaybePlainType ToNonLiteralType() => this;

    IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType);

    string ToString();
}
