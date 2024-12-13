using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[Closed(typeof(NonVoidType), typeof(IMaybeFunctionType))]
public interface IMaybeNonVoidType : IMaybeType
{
    new IMaybeNonVoidPlainType PlainType { get; }
    IMaybePlainType IMaybeType.PlainType => PlainType;
}
