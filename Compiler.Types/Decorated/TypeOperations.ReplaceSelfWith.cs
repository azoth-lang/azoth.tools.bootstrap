using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    /// <summary>
    /// Replace self viewpoint types using the given type as self.
    /// </summary>
    public static IMaybeType ReplaceSelfWith(this IMaybeType type, IMaybeType selfType)
    {
        // TODO what about CapabilitySetTypes?
        if (selfType is not CapabilityType selfReferenceType) return type;
        return type.ReplaceSelfWith(selfReferenceType.Capability);
    }

    /// <summary>
    /// Replace self viewpoint types using the given type as self.
    /// </summary>
    public static IMaybeType ReplaceSelfWith(this IMaybeType type, Capability capability)
    {
        return type switch
        {
            SelfViewpointType t => t.Referent.ReplaceSelfWith(capability).AccessedVia(capability),
            // TODO doesn't this need to apply to type arguments?
            //ReferenceType t => ReplaceSelfWith(t, capability),
            //OptionalType t => ReplaceSelfWith(t, capability),
            _ => type,
        };
    }
}
