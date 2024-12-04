using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// e.g. `self |> mut Foo` Applies to all non-void types
public sealed class SelfViewpointType : INonVoidType
{
    public CapabilitySet Capability { get; }

    public INonVoidType Referent { get; }

    public INonVoidPlainType PlainType => Referent.PlainType;

    public SelfViewpointType(CapabilitySet capability, INonVoidType referent)
    {
        Capability = capability;
        Referent = referent;
    }

    public override string ToString() => $"{Capability.ToILString()} self |> {Referent}";
}
