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
    LentIsolated,
    Mutable,
    LentMutable,
    ReadOnly, // read-only from this reference, possibly writable from others
    LentReadOnly,
    Constant,
    LentConstant,
    Identity
}

public static class DeclaredReferenceCapabilityExtensions
{
    public static ReferenceCapability ToReferenceCapability(this DeclaredReferenceCapability capability)
        => capability switch
        {
            DeclaredReferenceCapability.Isolated => ReferenceCapability.Isolated,
            DeclaredReferenceCapability.LentIsolated => ReferenceCapability.LentIsolated,

            DeclaredReferenceCapability.Mutable => ReferenceCapability.Mutable,
            DeclaredReferenceCapability.LentMutable => ReferenceCapability.LentMutable,

            DeclaredReferenceCapability.ReadOnly => ReferenceCapability.ReadOnly,
            DeclaredReferenceCapability.LentReadOnly => ReferenceCapability.LentReadOnly,

            DeclaredReferenceCapability.Constant => ReferenceCapability.Constant,
            DeclaredReferenceCapability.LentConstant => ReferenceCapability.LentConstant,

            DeclaredReferenceCapability.Identity => ReferenceCapability.Identity,
            _ => throw ExhaustiveMatch.Failed(capability),
        };
}
