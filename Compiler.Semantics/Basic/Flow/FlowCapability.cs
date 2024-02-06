using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// Tracks the original and modified capability for a reference.
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public readonly record struct FlowCapability(ReferenceCapability Original, CapabilityRestrictions Restrictions)
{
    public ReferenceCapability Modified { get; init; } = Original;

    public ReferenceCapability Current
        => Restrictions switch
        {
            CapabilityRestrictions.ReadWrite => ReferenceCapability.Identity,
            CapabilityRestrictions.Write => Modified.WithoutWrite(),
            CapabilityRestrictions.None => Modified,
            _ => throw ExhaustiveMatch.Failed(Restrictions)
        };

    public static implicit operator FlowCapability(ReferenceCapability capability)
        => new(capability, CapabilityRestrictions.None);

    public FlowCapability WhenAliased() => this with { Modified = Modified.WhenAliased() };

    public FlowCapability AfterFreeze() => this with { Modified = Modified.Freeze() };

    public FlowCapability With(ReferenceCapability capability) => this with { Modified = capability };
    public FlowCapability WithRestrictions(CapabilityRestrictions restrictions)
        => this with { Restrictions = restrictions };

    public override string ToString()
    {
        if (Original == Current)
            return Original.ToILString();

        var restricted = Restrictions == CapabilityRestrictions.None ? "" : "-/";
        return $"{Original.ToILString()} {restricted}-> {Current.ToILString()}";
    }
}
