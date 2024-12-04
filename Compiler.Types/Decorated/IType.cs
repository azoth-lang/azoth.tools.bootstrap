using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[Closed(typeof(INonVoidType), typeof(VoidType))]
public interface IType : IMaybeType
{
    new IPlainType PlainType { get; }
    IMaybePlainType IMaybeType.PlainType => PlainType;
}
