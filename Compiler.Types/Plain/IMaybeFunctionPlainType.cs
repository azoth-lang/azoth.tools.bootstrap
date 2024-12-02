using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(FunctionPlainType), typeof(UnknownPlainType))]
public interface IMaybeFunctionPlainType : IMaybeNonVoidPlainType;
