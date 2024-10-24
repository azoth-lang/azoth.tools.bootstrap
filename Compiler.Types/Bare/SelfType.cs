using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

public sealed class SelfType : BareTypeVariableType
{
    public override CapabilityType With(Capability capability)
        => throw new NotImplementedException();
}
