using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

internal static class SharingIsTrackedExtensions
{
    public static bool SharingIsTracked(this IBindingNode node)
    {
        // Any lent parameter needs tracked to prevent sharing with it
        if (node.IsLentBinding) return true;
        return SharingIsTracked(node.BindingType);
    }

    public static bool SharingIsTracked(this IMaybePseudotype pseudotype)
        => pseudotype.ToUpperBound().SharingIsTracked();

    public static bool SharingIsTracked(this IMaybeExpressionType type)
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
