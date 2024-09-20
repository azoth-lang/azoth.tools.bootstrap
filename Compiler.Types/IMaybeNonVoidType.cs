using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(INonVoidType), typeof(IMaybeFunctionType))]
public interface IMaybeNonVoidType : IMaybeType
{
}
