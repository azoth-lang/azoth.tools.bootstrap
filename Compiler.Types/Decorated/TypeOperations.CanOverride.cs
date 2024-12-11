using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    // TODO shouldn't even the self parameter have IsLent?
    public static bool CanOverride(this IType self, IType baseParameterType)
        => baseParameterType.IsSubtypeOf(self);

    public static bool CanOverride(this ParameterType self, ParameterType baseParameter)
        => baseParameter.IsLent.Implies(self.IsLent) && baseParameter.Type.IsSubtypeOf(self.Type);
}
