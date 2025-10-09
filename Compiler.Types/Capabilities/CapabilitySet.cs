using System.Collections.Frozen;
using System.Diagnostics;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.Capability;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class CapabilitySet : ICapabilityConstraint
{
    /// <summary>
    /// Any capability that is directly readable <i>without conversion</i> (i.e. `mut`, `const`, `temp const`, `read`).
    /// </summary>
    /// <remarks>The "without conversion" in the summary is meant to indicate that one must be able
    /// to read from the value without creating an alias with a different capability. Thus, it
    /// really means that this is all <see cref="Aliasable"/> capabilities that allow reading.</remarks>
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
    /// Any capability that does not allow writing (i.e. `const`, `temp const`, `read`, `id`).
    /// </summary>
    public static readonly CapabilitySet ReadOnly = new("readonly", Constant, TemporarilyConstant, Read, Identity);

    /// <summary>
    /// Any capability that is temporary (i.e. `temp iso`, `temp const`).
    /// </summary>
    // TODO `temporary` is a strange name because `temporary self` reads wrong. The adjective
    // "temporary" doesn't describe `self` it describes the capabilities. Maybe a capability that
    // indicates there are no other writers and also included `const` would be better?
    // Another option would be to name this `temp any` or `any temp`
    public static readonly CapabilitySet Temporary = new("temporary", TemporarilyIsolated, TemporarilyConstant);

    /// <summary>
    /// Any capability whatsoever (i.e. `iso`, `temp iso`, `own`, `mut`, `const`, `temp const`, `read`, `id`).
    /// </summary>
    public static readonly CapabilitySet Any
        = new("any", Isolated, TemporarilyIsolated, Mutable, Constant, TemporarilyConstant, Read, Identity);

    /// <summary>
    /// The default capability set used for generic type parameters that do not specify one.
    /// </summary>
    /// <remarks>This is just an alias for the default of <see cref="Aliasable"/> used to make code
    /// more readable.</remarks>
    public static readonly CapabilitySet GenericParameterDefault = Aliasable;

    public IReadOnlySet<Capability> AllowedCapabilities { [DebuggerStepThrough] get; }

    public bool AnyCapabilityAllowsWrite { [DebuggerStepThrough] get; }

    public Capability UpperBound { [DebuggerStepThrough] get; }

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

    public bool IsSubtypeOf(ICapabilityConstraint other) => other.IsAssignableFrom(this);

    public CapabilitySet Intersect(CapabilitySet other)
    {
        if (IsSubtypeOf(other)) return this;
        if (other.IsSubtypeOf(this)) return other;
        throw new NotImplementedException();
    }

    public override string ToString() => ToILString();

    public string ToILString() => name;

    public string ToSourceCodeString() => name;
}
