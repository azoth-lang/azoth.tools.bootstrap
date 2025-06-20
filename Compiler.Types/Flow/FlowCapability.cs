using System.Diagnostics;
using System.Runtime.InteropServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow;

/// <summary>
/// Tracks the original and modified capability for part of a reference.
/// </summary>
/// <remarks>Typically, this applies only to the outermost capability of a type. However, if a type
/// has independent type parameters, then this may apply to the type parameters as well.</remarks>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[StructLayout(LayoutKind.Auto)]
public readonly record struct FlowCapability
{
    public FlowCapability(Capability original)
    {
        Requires.NotNull(original, nameof(original));
        Original = original;
        Modified = original;
    }

    public Capability Original { get; }

    /// <summary>
    /// The capability after it has been modified by aliasing, freezing, etc.
    /// </summary>
    /// <remarks>This is not the actual capability at the moment. That is <see cref="Current"/>.</remarks>
    public Capability Modified { get; init; }

    /// <summary>
    /// Any temporary restrictions on the capability (i.e. from a temporary freeze or move).
    /// </summary>
    public CapabilityRestrictions Restrictions { get; init; } = CapabilityRestrictions.None;

    /// <summary>
    /// The current capability, taking into account modifications and restrictions.
    /// </summary>
    public Capability Current
        => Restrictions switch
        {
            CapabilityRestrictions.ReadWrite => Capability.Identity,
            CapabilityRestrictions.Write => Modified.WithoutWrite(),
            CapabilityRestrictions.None => Modified,
            _ => throw ExhaustiveMatch.Failed(Restrictions)
        };

    public static implicit operator FlowCapability(Capability capability)
        => new(capability);

    public FlowCapability WhenAliased() => this with { Modified = Modified.WhenAliased() };

    public FlowCapability OfAlias() => this with { Modified = Modified.OfAlias() };

    public FlowCapability AfterFreeze() => this with { Modified = Modified.Freeze() };

    public FlowCapability AfterMove() => this with { Modified = Capability.Identity };

    public FlowCapability With(Capability capability) => this with { Modified = capability };
    public FlowCapability WithRestrictions(CapabilityRestrictions restrictions)
        => this with { Restrictions = restrictions };

    public override string ToString()
    {
        if (Original is null
           && Modified is null
           && Restrictions == CapabilityRestrictions.None)
            return "N/A";

        if (Original == Current)
            return Original.ToILString();

        var restricted = Restrictions switch
        {
            CapabilityRestrictions.None => "",
            CapabilityRestrictions.Write => "-w ",
            CapabilityRestrictions.ReadWrite => "-rw ",
            _ => throw ExhaustiveMatch.Failed(Restrictions)
        };
        return $"{Original!.ToILString()} {restricted}-> {Current.ToILString()}";
    }

    public void Deconstruct(out Capability original) => original = Original;
}
