using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public interface IMaybeType
{
    IMaybePlainType PlainType { get; }
}
