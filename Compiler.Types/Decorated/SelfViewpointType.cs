using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// e.g. `self |> mut Foo` Applies to all non-void types
public sealed class SelfViewpointType : INonVoidType
{
    public INonVoidType Referent { get; }

    public INonVoidPlainType PlainType => Referent.PlainType;

    public SelfViewpointType(INonVoidType referent)
    {
        Referent = referent;
    }
}
