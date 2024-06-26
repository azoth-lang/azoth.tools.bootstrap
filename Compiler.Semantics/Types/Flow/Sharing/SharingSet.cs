using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

internal sealed class SharingSet : IReadOnlyCollection<IValue>, IEquatable<SharingSet>
{
    public static SharingSet Union(IReadOnlyCollection<SharingSet> sets)
    {
        if (sets.Count == 0)
            throw new ArgumentException("Cannot union no sets");
        if (sets.Count == 1)
            return sets.First();
        SharingSet first = sets.First();
        bool isLent = first.IsLent;
        var values = first.values.ToBuilder();
        foreach (var set in sets.Skip(1))
        {
            if (set.IsLent != isLent)
                throw new ArgumentException("Cannot union sets with different lent status");
            values.UnionWith(set.values);
        }
        return new SharingSet(isLent, values.ToImmutable());
    }

    public bool IsLent { get; }
    private readonly ImmutableHashSet<IValue> values;
    private readonly ImmutableHashSet<IConversion> conversions;

    public SharingSet(bool isLent, IValue value)
    {
        IsLent = isLent;
        values = [value];
        conversions = ImmutableHashSet<IConversion>.Empty;
    }

    public SharingSet(bool isLent, IValue value, params IValue?[] values)
    {
        IsLent = isLent;
        this.values = ImmutableHashSet.Create(values.WhereNotNull().Append(value).ToArray());
        conversions = ImmutableHashSet<IConversion>.Empty;
    }

    public SharingSet(bool isLent, IEnumerable<IValue> values)
        : this(isLent, values.ToImmutableHashSet(), ImmutableHashSet<IConversion>.Empty)
    {
    }

    public SharingSet(bool isLent, ResultValue value, IConversion conversion)
    {
        IsLent = isLent;
        values = [value];
        conversions = [conversion];
    }

    public SharingSet(SharingSet other)
        : this(other.IsLent, other.values)
    {
    }

    public SharingSet(bool isLent, ImmutableHashSet<IValue> values, ImmutableHashSet<IConversion> conversions)
    {
        if (values.IsEmpty)
            throw new ArgumentException("Sharing set must contain at least one value", nameof(values));
        IsLent = isLent;
        this.values = values;
        this.conversions = conversions;
    }

    public CapabilityRestrictions Restrictions
        => conversions.Select(c => c.RestrictionsImposed).Append(CapabilityRestrictions.None).Max();

    public SharingSet Declare(ResultValue value)
        => new(IsLent, values.Add(value));

    public SharingSet Replace(IValue oldValue, IValue newValue)
    {
        var builder = values.ToBuilder();
        builder.Remove(oldValue);
        builder.Add(newValue);
        return new(IsLent, builder.ToImmutable());
    }

    public SharingSet Replace(IEnumerable<IValue> oldValues, IValue newValue)
    {
        var builder = values.ToBuilder();
        builder.ExceptWith(oldValues);
        builder.Add(newValue);
        return new(IsLent, builder.ToImmutable());
    }

    public SharingSet Replace(IValue oldValue, IConversion conversion)
        => new(IsLent, values.Remove(oldValue), conversions.Add(conversion));

    // TODO what if this makes the set empty?
    public SharingSet Drop(IValue value)
        => new(IsLent, values.Remove(value));

    public IEnumerator<IValue> GetEnumerator() => values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => values.Count;

    public bool Contains(IValue item) => values.Contains(item);

    #region Equality
    public bool Equals(SharingSet? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return IsLent == other.IsLent
               && values.SetEquals(other.values)
               && conversions.SetEquals(other.conversions);
    }

    public override bool Equals(object? obj)
       => ReferenceEquals(this, obj) || obj is SharingSet other && Equals(other);

    public override int GetHashCode()
        // Note GetHashCode isn't implemented for ImmutableHashSet, so we don't include the values.
        // To do so, we'd need to sort them and the cache the hash code.
        => HashCode.Combine(IsLent, values.Count, conversions.Count);
    #endregion

    public override string ToString()
    {
        var result = $"Count {Count}";
        if (IsLent) result += ", lent";
        //var restrictions = Restrictions;
        //if (restrictions == CapabilityRestrictions.ReadWrite)
        //    result += ", restrict read/write";
        //else if (restrictions == CapabilityRestrictions.Write) result += ", restrict write";
        return result;
    }
}
