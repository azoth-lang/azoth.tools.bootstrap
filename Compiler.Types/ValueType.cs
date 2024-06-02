using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class ValueType<TDeclared> : CapabilityType<TDeclared>
    where TDeclared : DeclaredValueType
{
    internal ValueType(Capability capability, BareValueType<TDeclared> bareType)
        : base(capability, bareType)
    {
    }

    public override CapabilityType<TDeclared> With(Capability capability)
    {
        if (capability == Capability) return this;
        return new ValueType<TDeclared>(capability, (BareValueType<TDeclared>)BareType);
    }
}
