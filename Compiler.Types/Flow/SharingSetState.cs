using System.Collections.Immutable;
using Azoth.Tools.Bootstrap.Compiler.Types.Flow.Sharing;
using Azoth.Tools.Bootstrap.Framework.Collections;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow;

public readonly struct SharingSetState : IMergeable<SharingSetState>, IEquatable<SharingSetState>
{
    public bool IsLent { get; }
    public ImmutableHashSet<IConversion> Conversions { get; }
    public CapabilityRestrictions Restrictions
        => Conversions.Select(c => c.RestrictionsImposed).Append(CapabilityRestrictions.None).Max();

    public SharingSetState(bool isLent)
    {
        IsLent = isLent;
        Conversions = ImmutableHashSet<IConversion>.Empty;
    }

    public SharingSetState(bool isLent, IConversion conversion)
        : this(isLent, [conversion])
    {
    }

    public SharingSetState(bool isLent, ImmutableHashSet<IConversion> conversions)
    {
        IsLent = isLent;
        Conversions = conversions;
    }

    public SharingSetState Merge(SharingSetState other)
    {
        var conversions = other.Conversions.IsEmpty ? Conversions
            : (Conversions.IsEmpty ? other.Conversions : Conversions.Union(other.Conversions));
        return new(IsLent || other.IsLent, conversions);
    }

    public SharingSetState Add(IConversion conversion)
        => new(IsLent, Conversions.Add(conversion));

    public SharingSetState Remove(IEnumerable<IConversion> conversions)
        => new(IsLent, Conversions.Except(conversions));

    #region Equality
    public bool Equals(SharingSetState other)
        => IsLent == other.IsLent && Conversions.SetEquals(other.Conversions);

    public override bool Equals(object? obj) => obj is SharingSetState other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(IsLent, Conversions.Count);
    #endregion
}
