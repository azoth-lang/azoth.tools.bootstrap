using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public class SharingSet : IReadOnlySharingSet
{
    /// <summary>
    /// Whether this sharing set contains lent references that must not be mixed with other sharing
    /// sets.
    /// </summary>
    public bool IsLent { get; }
    private readonly HashSet<ISharingVariable> variables;
    public int Count => variables.Count;
    public CapabilityRestrictions Restrictions
        => variables.Select(v => v.RestrictionsImposed).Append(CapabilityRestrictions.None).Max();
    public bool IsIsolated => variables.Count == 1;
    public bool IsAlive => variables.Any(v => v.KeepsSetAlive);

    public SharingSet(bool isLent, IReadOnlySet<ISharingVariable> variables)
    {
        IsLent = isLent;
        this.variables = variables.ToHashSet();
    }

    public SharingSet(IReadOnlySharingSet set)
    {
        IsLent = set.IsLent;
        variables = set.ToHashSet();
    }

    public SharingSet(ISharingVariable variable, bool isLent)
    {
        IsLent = isLent;
        variables = new HashSet<ISharingVariable> { variable };
    }

    public bool IsIsolatedExceptFor(ResultVariable result)
        => variables.Count <= 2 && variables.Except(result).Count() == 1;

    public IEnumerator<ISharingVariable> GetEnumerator() => variables.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public SharingSetSnapshot Snapshot()
        => new(IsLent, variables.ToFixedSet());

    public void Declare(ExternalReference lentGroup)
    {
        if (!IsLent) throw new InvalidOperationException("Cannot declare lent group for non-lent sharing set.");

        variables.Add(lentGroup);
    }

    public void Declare(ResultVariable variable) => variables.Add(variable);

    public void Remove(ISharingVariable variable) => variables.Remove(variable);

    public void UnionWith(SharingSet smallerSet)
    {
        if ((IsLent && !smallerSet.IsIsolated)
            || (smallerSet.IsLent && !IsIsolated))
            throw new InvalidOperationException("Cannot union two sharing sets if either is lent unless they are isolated sets.");
        variables.UnionWith(smallerSet.variables);
    }

    public void Clear() => variables.Clear();

    #region Equals
    public bool Equals(SharingSet? other)
        => other is not null && IsLent == other.IsLent && variables.Equals(other.variables);

    public override bool Equals(object? obj) => obj is SharingSet other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(IsLent, variables);

    public static bool operator ==(SharingSet left, SharingSet right) => left.Equals(right);

    public static bool operator !=(SharingSet left, SharingSet right) => !left.Equals(right);
    #endregion

    public override string ToString()
    {
        var result = $"Count {variables.Count}";
        if (IsLent) result += ", lent";
        var restrictions = Restrictions;
        if (restrictions == CapabilityRestrictions.ReadWrite) result += ", restrict read/write";
        else if (restrictions == CapabilityRestrictions.Write) result += ", restrict write";
        return result;
    }
}
