using Azoth.Tools.Bootstrap.Framework;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.Capability;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

public sealed class CapabilitySetWithIdentity : CapabilitySet
{
    /// <summary>
    /// Any capability that can be shared between more than one thread (i.e. `const`, `id`).
    /// </summary>
    public new static readonly CapabilitySetWithIdentity Shareable = new("shareable", Constant, Identity);

    /// <summary>
    /// Any capability that can alias itself (i.e. `mut`, `const`, `temp const`, `read`, `id`).
    /// </summary>
    /// <remarks>This is the default capability constraint for type parameters.</remarks>
    public new static readonly CapabilitySetWithIdentity Aliasable
        = new("aliasable", Mutable, Constant, TemporarilyConstant, Read, Identity);

    /// <summary>
    /// Any capability that can be sent between threads (i.e. `iso`, `const`, `id`).
    /// </summary>
    public new static readonly CapabilitySetWithIdentity Sendable = new("sendable", Isolated, Constant, Identity);

    /// <summary>
    /// Any capability that does not allow writing (i.e. `const`, `temp const`, `read`, `id`).
    /// </summary>
    public new static readonly CapabilitySetWithIdentity ReadOnly
        = new("readonly", Constant, TemporarilyConstant, Read, Identity);

    /// <summary>
    /// Any capability whatsoever (i.e. `iso`, `temp iso`, `own`, `mut`, `const`, `temp const`, `read`, `id`).
    /// </summary>
    public new static readonly CapabilitySetWithIdentity Any
        = new("any", Isolated, TemporarilyIsolated, Mutable, Constant, TemporarilyConstant, Read, Identity);

    private CapabilitySetWithIdentity(string name, params Capability[] allowedCapabilities)
        : base(name, allowedCapabilities)
    {
        Requires.That(UpperBound == Identity, nameof(allowedCapabilities), "Must contain `id`");
    }

    public CapabilitySetWithIdentity Intersect(CapabilitySetWithIdentity other)
    {
        if (IsSubtypeOf(other)) return this;
        if (other.IsSubtypeOf(this)) return other;
        // Given the current list of capability sets containing identity, the intersection is always
        // `shareable` if it isn't one of the two capability sets. If more capability sets are added
        // this may change.
        return Shareable;
    }
}
