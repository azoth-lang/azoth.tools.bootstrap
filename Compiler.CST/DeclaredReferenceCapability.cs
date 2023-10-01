using Azoth.Tools.Bootstrap.Compiler.Types;
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
    Mutable,
    ReadOnly, // read-only from this reference, possibly writable from others
    Constant,
    Identity
}

public static class DeclaredReferenceCapabilityExtensions
{
    public static ReferenceCapability ToReferenceCapability(this DeclaredReferenceCapability capability)
        => capability switch
        {
            DeclaredReferenceCapability.Isolated => ReferenceCapability.Isolated,

            DeclaredReferenceCapability.Constant => ReferenceCapability.Constant,
            DeclaredReferenceCapability.Identity => ReferenceCapability.Identity,

            DeclaredReferenceCapability.Mutable => ReferenceCapability.Mutable,
            DeclaredReferenceCapability.ReadOnly => ReferenceCapability.ReadOnly,
            _ => throw ExhaustiveMatch.Failed(capability),
        };
}
