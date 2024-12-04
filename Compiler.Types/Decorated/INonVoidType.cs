using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public interface INonVoidType : IType
{
    new INonVoidPlainType PlainType { get; }
    IPlainType IType.PlainType => PlainType;
}
