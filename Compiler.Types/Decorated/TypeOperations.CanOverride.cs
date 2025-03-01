using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    // TODO shouldn't even the self parameter have IsLent?
    public static bool CanOverride(this Type self, Type baseParameterType)
        => baseParameterType.IsSubtypeOf(self, substitutable: true);

    public static bool CanOverride(this ParameterType self, ParameterType baseParameter)
    {
        if (!baseParameter.Type.IsSubtypeOf(self.Type, substitutable: true))
            return false;
        if (baseParameter.IsLent.Implies(self.IsLent))
            return true;

        if (self.Type is CapabilityType parameterType)
            // TODO what about independent generic parameters
            return CapabilitySet.Shareable.AllowedCapabilities.Contains(parameterType.Capability);

        return false;
    }
}
