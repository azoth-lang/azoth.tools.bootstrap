using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(NonVoidPlainType),
    typeof(IMaybeFunctionPlainType))]
public interface IMaybeNonVoidPlainType : IMaybePlainType
{
    /// <summary>
    /// Convert types for literals (e.g. <c>bool[true]</c>, <c>int[42]</c> etc.) to their
    /// corresponding types.
    /// </summary>
    new IMaybeNonVoidPlainType ToNonLiteral();
    IMaybePlainType IMaybePlainType.ToNonLiteral() => ToNonLiteral();
}
