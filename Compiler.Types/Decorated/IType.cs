using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public interface IType : IMaybeType
{
    new IPlainType PlainType { get; }
    IMaybePlainType IMaybeType.PlainType => PlainType;
}
