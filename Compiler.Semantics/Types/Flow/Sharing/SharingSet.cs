using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

internal sealed class SharingSet : IReadOnlyCollection<IValue>, IEquatable<SharingSet>
{
    /// <summary>
    /// Union multiple sharing sets into a single set.
    /// </summary>
    /// <remarks>All sets must either be lent or non-lent.</remarks>
    public static SharingSet Union(IReadOnlyCollection<SharingSet> sets)
    {
        if (sets.Count == 0)
            throw new ArgumentException("Cannot union no sets");
        if (sets.Count == 1)
            return sets.First();
        SharingSet first = sets.First();
        bool isLent = first.IsLent;
        var values = first.values.ToBuilder();
        var conversions = first.Conversions.ToBuilder();
        foreach (var set in sets.Skip(1))
        {
            if (set.IsLent != isLent)
                throw new ArgumentException("Cannot union sets with different lent status");
            values.UnionWith(set.values);
            conversions.UnionWith(set.Conversions);
        }
        return new(isLent, values.ToImmutable(), conversions.ToImmutable());
    }

    /// <summary>
    /// Combine all the sharing sets for arguments into a single set with the return value id and
    /// drop the values for all arguments.
    /// </summary>
    /// <remarks>Only non-lent sets are combined, but that is handled by the caller</remarks>
    public static SharingSet CombineArguments(
        IReadOnlyCollection<SharingSet> argumentSets,
        ValueId returnValueId,
        IEnumerable<ResultValue> argumentValues)
    {
        var returnValue = ResultValue.Create(returnValueId);
        if (argumentSets.Count == 0)
            return Declare(false, returnValue);

        SharingSet first = argumentSets.First();
        bool isLent = first.IsLent;
        var values = first.values.ToBuilder();
        var conversions = first.Conversions.ToBuilder();
        foreach (var set in argumentSets.Skip(1))
        {
            isLent |= set.IsLent;
            values.UnionWith(set.values);
            conversions.UnionWith(set.Conversions);
        }
        values.ExceptWith(argumentValues);
        values.Add(returnValue);

        return new(isLent, values.ToImmutable(), conversions.ToImmutable());
    }

    public bool IsLent { get; }
    private readonly ImmutableHashSet<IValue> values;
    public ImmutableHashSet<IConversion> Conversions { get; }

    public static SharingSet Declare(bool isLent, IValue value)
        => new(isLent, [value], ImmutableHashSet<IConversion>.Empty);

    public static SharingSet Declare(bool isLent, IValue value, params IValue?[] values)
        => new(isLent, values.WhereNotNull().Append(value).ToImmutableHashSet(), ImmutableHashSet<IConversion>.Empty);

    public static SharingSet Declare(bool isLent, IEnumerable<IValue> values)
        => new(isLent, values.ToImmutableHashSet(), ImmutableHashSet<IConversion>.Empty);

    public static SharingSet DeclareConversion(bool isLent, ResultValue value, IConversion conversion)
        => new(isLent, [value], [conversion]);

    private SharingSet(bool isLent, ImmutableHashSet<IValue> values, ImmutableHashSet<IConversion> conversions)
    {
        if (values.IsEmpty)
            throw new ArgumentException("Sharing set must contain at least one value", nameof(values));
        IsLent = isLent;
        this.values = values;
        Conversions = conversions;
    }

    public CapabilityRestrictions Restrictions
        => Conversions.Select(c => c.RestrictionsImposed).Append(CapabilityRestrictions.None).Max();

    public SharingSet Declare(ResultValue value)
        => new(IsLent, values.Add(value), Conversions);

    public SharingSet Replace(IValue oldValue, IValue newValue)
    {
        var builder = values.ToBuilder();
        builder.Remove(oldValue);
        builder.Add(newValue);
        return new(IsLent, builder.ToImmutable(), Conversions);
    }

    public SharingSet Replace(IEnumerable<IValue> oldValues, IValue newValue)
    {
        var builder = values.ToBuilder();
        builder.ExceptWith(oldValues);
        builder.Add(newValue);
        return new(IsLent, builder.ToImmutable(), Conversions);
    }

    public SharingSet? Replace(IValue oldValue, IConversion conversion)
    {
        var newValues = values.Remove(oldValue);
        if (newValues.IsEmpty) return null;
        return new(IsLent, newValues, Conversions.Add(conversion));
    }

    public SharingSet? Drop(IValue value)
    {
        var newValues = values.Remove(value);
        if (newValues.IsEmpty) return null;
        return new(IsLent, newValues, Conversions);
    }

    public SharingSet? Drop(IEnumerable<IValue> values)
    {
        var builder = this.values.ToBuilder();
        builder.ExceptWith(values);
        var newValues = builder.ToImmutable();
        if (newValues.IsEmpty) return null;
        return new(IsLent, newValues, Conversions);
    }

    public SharingSet Drop(IEnumerable<IConversion> conversions)
    {
        var builder = Conversions.ToBuilder();
        builder.ExceptWith(conversions);
        var newConversions = builder.ToImmutable();
        return new(IsLent, values, newConversions);
    }

    public IEnumerator<IValue> GetEnumerator() => values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => values.Count;

    public bool Contains(IValue item) => values.Contains(item);

    #region Equality
    public bool Equals(SharingSet? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return IsLent == other.IsLent
               && values.SetEquals(other.values)
               && Conversions.SetEquals(other.Conversions);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is SharingSet other && Equals(other);

    public override int GetHashCode()
        // Note GetHashCode isn't implemented for ImmutableHashSet, so we don't include the values.
        // To do so, we'd need to sort them and the cache the hash code.
        => HashCode.Combine(IsLent, values.Count, Conversions.Count);
    #endregion

    public override string ToString()
    {
        var result = $"Count {Count}";
        if (IsLent) result += ", lent";
        var restrictions = Restrictions;
        switch (restrictions)
        {
            case CapabilityRestrictions.ReadWrite:
                result += ", restrict read/write";
                break;
            case CapabilityRestrictions.Write:
                result += ", restrict write";
                break;
        }
        return result;
    }
}
