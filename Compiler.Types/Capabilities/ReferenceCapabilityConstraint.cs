using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.ReferenceCapability;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

public sealed class ReferenceCapabilityConstraint : IReferenceCapabilityConstraint
{
    /// <summary>
    /// Any capability that is directly readable without conversion (i.e. `mut`, `const`, `temp const`, `read`).
    /// </summary>
    public static readonly ReferenceCapabilityConstraint Readable
        = new("readable", Mutable, Constant, TemporarilyConstant, Read);

    /// <summary>
    /// Any capability that can be shared between more than one thread (i.e. `const`, `id`).
    /// </summary>
    public static readonly ReferenceCapabilityConstraint Shareable
        = new("shareable", Constant, Identity);

    /// <summary>
    /// Any capability that can alias itself (i.e. `mut`, `const`, `temp const`, `read`, `id`).
    /// </summary>
    /// <remarks>This is the default capability constraint for type parameters.</remarks>
    public static readonly ReferenceCapabilityConstraint Aliasable
        = new("aliasable", Mutable, Constant, TemporarilyConstant, Read, Identity);

    /// <summary>
    /// Any capability that can be sent between threads (i.e. `iso`, `const`, `id`).
    /// </summary>
    public static readonly ReferenceCapabilityConstraint Sendable
        = new("sendable", Isolated, Constant, Identity);

    /// <summary>
    /// Any capability whatsoever (i.e. `iso`, `temp iso`, `mut`, `const`, `temp const`, `read`, `id`).
    /// </summary>
    public static readonly ReferenceCapabilityConstraint Any
        = new("any", Isolated, TemporarilyIsolated, Mutable, Constant, TemporarilyConstant, Read, Identity);

    public IReadOnlySet<ReferenceCapability> AllowedCapabilities { get; }

    public bool AnyCapabilityAllowsWrite { get; }

    private ReferenceCapabilityConstraint(string name, params ReferenceCapability[] allowedCapabilities)
    {
        this.name = name;
        AllowedCapabilities = allowedCapabilities.ToFrozenSet();
        AnyCapabilityAllowsWrite = AllowedCapabilities.Any(capability => capability.AllowsWrite);
    }

    private readonly string name;

    public bool IsAssignableFrom(IReferenceCapabilityConstraint from)
        => from switch
        {
            ReferenceCapability capability => AllowedCapabilities.Contains(capability),
            ReferenceCapabilityConstraint constraint
                => AllowedCapabilities.IsSupersetOf(constraint.AllowedCapabilities),
            _ => throw ExhaustiveMatch.Failed(from)
        };

    public override string ToString() => name;
}
