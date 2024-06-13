using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

internal sealed class SharingSet : IReadOnlyCollection<IValue>, IEquatable<SharingSet>
{
    public bool IsLent { get; }
    private readonly ImmutableHashSet<IValue> values;

    public SharingSet(bool isLent, IValue value, params IValue?[] values)
    {
        IsLent = isLent;
        this.values = ImmutableHashSet.Create(values.WhereNotNull().Append(value).ToArray());
    }

    public SharingSet(bool isLent, IEnumerable<IValue> values)
        : this(isLent, values.ToImmutableHashSet())
    {
    }

    private SharingSet(bool isLent, ImmutableHashSet<IValue> values)
    {
        IsLent = isLent;
        this.values = values;
    }

    public SharingSet Declare(ResultValue result)
        => new(IsLent, values.Add(result));

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
        return IsLent == other.IsLent && values.Equals(other.values);
    }

    public override bool Equals(object? obj)
       => ReferenceEquals(this, obj) || obj is SharingSet other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(IsLent, values);
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
