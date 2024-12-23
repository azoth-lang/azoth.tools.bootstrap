using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

public static class SharingIsTrackedExtensions
{
    public static bool SharingIsTracked(this IMaybeType type)
    {
        // TODO this isn't correct since optional types and function types need tracked

        // If it isn't a capability type, then no need to track it
        if (type is not CapabilityType { Capability: var capability })
            return false;

        return capability.SharingIsTracked();
    }

    public static bool SharingIsTracked(this Capability capability)
        // Constant and Identity capabilities never need tracked
        => capability != Capability.Constant && capability != Capability.Identity;
}
