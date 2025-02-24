using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    // TODO shouldn't even the self parameter have IsLent?
    public static bool CanOverride(this Type self, Type baseParameterType)
        => baseParameterType.IsSubtypeOf(self);

    public static bool CanOverride(this ParameterType self, ParameterType baseParameter)
    {
        if (baseParameter.IsLent.Implies(self.IsLent) && baseParameter.Type.IsSubtypeOf(self.Type))
            return true;

        if (baseParameter.IsLent && !self.IsLent && baseParameter.Type.IsSubtypeOf(self.Type)
            && self.Type is CapabilityType parameterType)
            // TODO what about independent generic parameters
            return CapabilitySet.Shareable.AllowedCapabilities.Contains(parameterType.Capability);

        return false;
    }
}
