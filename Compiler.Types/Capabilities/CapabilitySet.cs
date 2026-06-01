using System.Collections.Frozen;
using System.Diagnostics;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.Capability;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
[Closed(typeof(CapabilitySetWithIdentity))]
public class CapabilitySet : ICapabilityConstraint
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
    public static readonly CapabilitySetWithIdentity Shareable = CapabilitySetWithIdentity.Shareable;

    /// <summary>
    /// Any capability that can alias itself (i.e. `mut`, `const`, `temp const`, `read`, `id`).
    /// </summary>
    /// <remarks>This is the default capability constraint for type parameters.</remarks>
    public static readonly CapabilitySetWithIdentity Aliasable = CapabilitySetWithIdentity.Aliasable;

    /// <summary>
    /// Any capability that can be sent between threads (i.e. `iso`, `const`, `id`).
    /// </summary>
    public static readonly CapabilitySetWithIdentity Sendable = CapabilitySetWithIdentity.Sendable;

    /// <summary>
    /// Any capability that does not allow writing (i.e. `const`, `temp const`, `read`, `id`).
    /// </summary>
    public static readonly CapabilitySetWithIdentity ReadOnly = CapabilitySetWithIdentity.ReadOnly;

    /// <summary>
    /// Any capability that is temporary (i.e. `temp iso`, `temp const`).
    /// </summary>
    // TODO `temporary` is a strange name because `temporary self` reads wrong. The adjective
    // "temporary" doesn't describe `self` it describes the capabilities. Maybe a capability that
    // indicates there are no other writers and also included `const` would be better?
    // Another option would be to name this `temp any` or `any temp`
    // TODO is this capability set even needed?
    public static readonly CapabilitySet Temporary = new("temporary", TemporarilyIsolated, TemporarilyConstant);

    /// <summary>
    /// Any capability whatsoever (i.e. `iso`, `temp iso`, `own`, `mut`, `const`, `temp const`, `read`, `id`).
    /// </summary>
    public static readonly CapabilitySetWithIdentity Any = CapabilitySetWithIdentity.Any;

    /// <summary>
    /// The default capability set used for generic type parameters that do not specify one.
    /// </summary>
    /// <remarks>This is just an alias for the default of <see cref="Aliasable"/> used to make code
    /// more readable.</remarks>
    public static readonly CapabilitySet GenericParameterDefault = Aliasable;

    public IReadOnlySet<Capability> AllowedCapabilities { [DebuggerStepThrough] get; }

    public bool SomeCapabilityAllowsWrite { [DebuggerStepThrough] get; }

    public Capability UpperBound { [DebuggerStepThrough] get; }

    protected CapabilitySet(string name, params Capability[] allowedCapabilities)
    {
        this.name = name;
        AllowedCapabilities = allowedCapabilities.ToFrozenSet();
        SomeCapabilityAllowsWrite = AllowedCapabilities.Any(capability => capability.AllowsWrite);
        UpperBound = AllowedCapabilities.Max(SubtypeComparer) ?? throw new ArgumentException("Must be at least one capability", nameof(allowedCapabilities));
    }

    private readonly string name;

    bool ICapabilityConstraint.IsSubsetOf(Capability other) => false;

    public bool IsSubsetOf(CapabilitySet other)
        => AllowedCapabilities.IsSubsetOf(other.AllowedCapabilities);

    /// <summary>
    /// Is this capability set a subtype of the given capability set?
    /// </summary>
    public bool IsSubtypeOf(CapabilitySet other)
        => AllowedCapabilities.IsSubsetOf(other.AllowedCapabilities);

    /// <summary>
    /// Is this capability set a subtype of the given capability? This occurs when all capabilities
    /// in the set are a subtype of the given capability.
    /// </summary>
    public bool IsSubtypeOf(Capability other) => UpperBound.IsSubtypeOf(other);

    public override string ToString() => ToILString();

    public string ToILString() => name;

    public string ToSourceCodeString() => name;
}
