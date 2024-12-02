using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(INonVoidPlainType),
    typeof(IMaybeFunctionPlainType))]
public interface IMaybeNonVoidPlainType : IMaybePlainType
{
}
