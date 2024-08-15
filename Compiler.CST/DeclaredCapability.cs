using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

/// <summary>
/// The reference capability a type was declared with.
/// </summary>
/// <remarks>This is distinct from <see cref="Capability"/> because that type supports
/// additional capabilities that cannot be directly declared.</remarks>
public enum DeclaredCapability
{
    Isolated = 1,
    TemporarilyIsolated,
    Mutable,
    Read, // read-only from this reference, possibly writable from others
    Constant,
    TemporarilyConstant,
    Identity
}

public static class DeclaredCapabilityExtensions
{
    public static Capability ToCapability(this DeclaredCapability capability)
        => capability switch
        {
            DeclaredCapability.Isolated => Capability.Isolated,
            DeclaredCapability.TemporarilyIsolated => Capability.TemporarilyIsolated,
            DeclaredCapability.Mutable => Capability.Mutable,
            DeclaredCapability.Read => Capability.Read,
            DeclaredCapability.Constant => Capability.Constant,
            DeclaredCapability.TemporarilyConstant => Capability.TemporarilyConstant,
            DeclaredCapability.Identity => Capability.Identity,
            _ => throw ExhaustiveMatch.Failed(capability),
        };

    public static Capability ToSelfParameterCapability(this DeclaredCapability declaredCapability)
        => declaredCapability switch
        {
            DeclaredCapability.Read => Capability.InitReadOnly,
            DeclaredCapability.Mutable => Capability.InitMutable,
            DeclaredCapability.Isolated => Capability.InitMutable,
            DeclaredCapability.TemporarilyIsolated => Capability.InitMutable,
            DeclaredCapability.Constant => Capability.InitReadOnly,
            DeclaredCapability.TemporarilyConstant => Capability.InitReadOnly,
            DeclaredCapability.Identity => Capability.InitReadOnly,
            _ => throw ExhaustiveMatch.Failed(declaredCapability)
        };
}
