using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// e.g. `readable Self`
// Applies to SelfPlainType
// Could be CapabilitySelfType and operate on an ICapabilityConstraint but that
// would introduce two ways of having types with capabilities on them.
public sealed class CapabilitySetSelfType
{
    public CapabilitySet Capability { get; }
    public SelfPlainType PlainType { get; }

    public CapabilitySetSelfType(CapabilitySet capability, SelfPlainType plainType)
    {
        Capability = capability;
        PlainType = plainType;
    }
}
