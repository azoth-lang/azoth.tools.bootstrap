using System.Collections.Frozen;
using System.Diagnostics;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.Capability;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

[DebuggerDisplay("{ToILString()}")]
public sealed class CapabilitySet : ICapabilityConstraint
{
    /// <summary>
    /// Any capability that is directly readable without conversion (i.e. `mut`, `const`, `temp const`, `read`).
    /// </summary>
    public static readonly CapabilitySet Readable
        = new("readable", Mutable, Constant, TemporarilyConstant, Read);

    /// <summary>
    /// Any capability that can be shared between more than one thread (i.e. `const`, `id`).
    /// </summary>
    public static readonly CapabilitySet Shareable
        = new("shareable", Constant, Identity);

    /// <summary>
    /// Any capability that can alias itself (i.e. `mut`, `const`, `temp const`, `read`, `id`).
    /// </summary>
    /// <remarks>This is the default capability constraint for type parameters.</remarks>
    public static readonly CapabilitySet Aliasable
        = new("aliasable", Mutable, Constant, TemporarilyConstant, Read, Identity);

    /// <summary>
    /// Any capability that can be sent between threads (i.e. `iso`, `const`, `id`).
    /// </summary>
    public static readonly CapabilitySet Sendable
        = new("sendable", Isolated, Constant, Identity);

    /// <summary>
    /// Any capability whatsoever (i.e. `iso`, `temp iso`, `mut`, `const`, `temp const`, `read`, `id`).
    /// </summary>
    public static readonly CapabilitySet Any
        = new("any", Isolated, TemporarilyIsolated, Mutable, Constant, TemporarilyConstant, Read, Identity);

    public IReadOnlySet<Capability> AllowedCapabilities { get; }

    public bool AnyCapabilityAllowsWrite { get; }

    public Capability UpperBound { get; }

    private CapabilitySet(string name, params Capability[] allowedCapabilities)
    {
        this.name = name;
        AllowedCapabilities = allowedCapabilities.ToFrozenSet();
        AnyCapabilityAllowsWrite = AllowedCapabilities.Any(capability => capability.AllowsWrite);
        // This does assume there will be a unique upper bound
        var upperBound = AllowedCapabilities.First();
        foreach (var allowedCapability in AllowedCapabilities)
        {
            if (!upperBound.IsAssignableFrom(allowedCapability))
                upperBound = allowedCapability;
        }
        UpperBound = upperBound;
    }

    private readonly string name;

    public bool IsAssignableFrom(ICapabilityConstraint from)
        => from switch
        {
            Capability capability => AllowedCapabilities.Contains(capability),
            CapabilitySet constraint
                => AllowedCapabilities.IsSupersetOf(constraint.AllowedCapabilities),
            _ => throw ExhaustiveMatch.Failed(from)
        };

    public override string ToString() => throw new NotSupportedException();

    public string ToILString() => name;

    public string ToSourceCodeString() => name;
}
