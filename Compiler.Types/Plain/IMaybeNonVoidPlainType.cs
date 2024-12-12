using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(NonVoidPlainType),
    typeof(IMaybeFunctionPlainType))]
public interface IMaybeNonVoidPlainType : IMaybePlainType
{
}
