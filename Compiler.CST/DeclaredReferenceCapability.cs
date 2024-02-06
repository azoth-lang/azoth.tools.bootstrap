using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

/// <summary>
/// The reference capability a type was declared with.
/// </summary>
/// <remarks>This is distinct from <see cref="ReferenceCapability"/> because that type supports
/// additional capabilities that cannot be directly declared.</remarks>
public enum DeclaredReferenceCapability
{
    Isolated = 1,
    TemporarilyIsolated,
    Mutable,
    ReadOnly, // read-only from this reference, possibly writable from others
    Constant,
    TemporarilyConstant,
    Identity
}

public static class DeclaredReferenceCapabilityExtensions
{
    public static ReferenceCapability ToReferenceCapability(this DeclaredReferenceCapability capability)
        => capability switch
        {
            DeclaredReferenceCapability.Isolated => ReferenceCapability.Isolated,
            DeclaredReferenceCapability.TemporarilyIsolated => ReferenceCapability.TemporarilyIsolated,
            DeclaredReferenceCapability.Mutable => ReferenceCapability.Mutable,
            DeclaredReferenceCapability.ReadOnly => ReferenceCapability.ReadOnly,
            DeclaredReferenceCapability.Constant => ReferenceCapability.Constant,
            DeclaredReferenceCapability.TemporarilyConstant => ReferenceCapability.TemporarilyConstant,
            DeclaredReferenceCapability.Identity => ReferenceCapability.Identity,
            _ => throw ExhaustiveMatch.Failed(capability),
        };
}
