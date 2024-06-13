using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

internal static class SharingIsTrackedExtensions
{
    public static bool SharingIsTracked(this IBindingNode node)
    {
        // Any lent parameter needs tracked to prevent sharing with it
        if (node.IsLentBinding) return true;
        return SharingIsTracked(node.BindingType);
    }

    /// <summary>
    /// Whether value sharing is tracked for this parameter type.
    /// </summary>
    public static bool SharingIsTracked(this ParameterType parameter)
    {
        // Any lent parameter needs tracked to prevent sharing with it
        if (parameter.IsLent) return true;
        return SharingIsTracked(parameter.Type);
    }

    /// <summary>
    /// Whether value sharing is tracked for this parameter type.
    /// </summary>
    public static bool SharingIsTracked(this SelfParameterType parameter)
    {
        // Any lent parameter needs tracked to prevent sharing with it
        if (parameter.IsLent) return true;
        return SharingIsTracked(parameter.Type);
    }

    public static bool SharingIsTracked(this Pseudotype pseudotype)
        => pseudotype.ToUpperBound().SharingIsTracked();

    public static bool SharingIsTracked(this DataType pseudotype)
    {
        // TODO this isn't correct since optional types and function types need tracked

        // If it isn't a capability type, then no need to track it
        if (pseudotype is not CapabilityType { Capability: var capability })
            return false;

        // Constant and Identity capabilities never need tracked
        return capability != Capability.Constant && capability != Capability.Identity;
    }

    public static bool SharingIsTracked(this IBindingNode node, FlowCapability flowCapability)
    {
        if (flowCapability.Modified == Capability.Identity) return false;
        return node.SharingIsTracked();
    }
}
