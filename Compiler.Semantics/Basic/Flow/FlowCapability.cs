using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// Tracks the original and modified capability for part of a reference.
/// </summary>
/// <remarks>Typically, this applies only to the outermost capability of a type. However, if a type
/// has independent type parameters, then this may apply to the type parameters as well.</remarks>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public readonly record struct FlowCapability(ReferenceCapability Original)
{
    /// <summary>
    /// The capability after it has been modified by aliasing, freezing, etc.
    /// </summary>
    /// <remarks>This is not the actual capability at the moment. That is <see cref="Current"/>.</remarks>
    public ReferenceCapability Modified { get; init; } = Original;

    /// <summary>
    /// Any temporary restrictions on the capability (i.e. from a temporarily freeze or move).
    /// </summary>
    public CapabilityRestrictions Restrictions { get; init; } = CapabilityRestrictions.None;

    /// <summary>
    /// The current capability, taking into account modifications and restrictions.
    /// </summary>
    public ReferenceCapability Current
        => Restrictions switch
        {
            CapabilityRestrictions.ReadWrite => ReferenceCapability.Identity,
            CapabilityRestrictions.Write => Modified.WithoutWrite(),
            CapabilityRestrictions.None => Modified,
            _ => throw ExhaustiveMatch.Failed(Restrictions)
        };

    public static implicit operator FlowCapability(ReferenceCapability capability)
        => new(capability);

    public FlowCapability WhenAliased() => this with { Modified = Modified.WhenAliased() };

    public FlowCapability OfAlias() => this with { Modified = Modified.OfAlias() };

    public FlowCapability AfterFreeze() => this with { Modified = Modified.Freeze() };

    public FlowCapability AfterMove() => this with { Modified = ReferenceCapability.Identity };

    public FlowCapability With(ReferenceCapability capability) => this with { Modified = capability };
    public FlowCapability WithRestrictions(CapabilityRestrictions restrictions)
        => this with { Restrictions = restrictions };

    public override string ToString()
    {
        if (Original == Current)
            return Original.ToILString();

        var restricted = Restrictions switch
        {
            CapabilityRestrictions.None => "",
            CapabilityRestrictions.Write => "-w ",
            CapabilityRestrictions.ReadWrite => "-rw ",
            _ => throw ExhaustiveMatch.Failed(Restrictions)
        };
        return $"{Original.ToILString()} {restricted}-> {Current.ToILString()}";
    }
}
