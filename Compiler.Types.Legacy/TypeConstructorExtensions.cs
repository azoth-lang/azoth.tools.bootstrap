using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

internal static class TypeConstructorExtensions
{
    public static CapabilityType? TryConstructConstNullary(this TypeConstructor typeConstructor)
    {
        if (!typeConstructor.Parameters.IsEmpty) return null;
        return new(Capability.Constant, new BareNonVariableType(typeConstructor, []));
    }
}
