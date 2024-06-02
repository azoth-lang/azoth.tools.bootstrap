using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class ReferenceType<TDeclared> : CapabilityType<TDeclared>
    where TDeclared : DeclaredReferenceType
{
    internal ReferenceType(Capability capability, BareReferenceType<TDeclared> bareType)
        : base(capability, bareType)
    {
    }

    public override CapabilityType<TDeclared> With(Capability capability)
    {
        if (capability == Capability) return this;
        return new ReferenceType<TDeclared>(capability, (BareReferenceType<TDeclared>)BareType);
    }
}
