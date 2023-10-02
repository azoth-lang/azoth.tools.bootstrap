using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public class SharingSet : IReadOnlySharingSet
{
    public bool IsLent { get; }
    private readonly HashSet<ISharingVariable> variables;
    private readonly HashSet<ISharingVariable> variablesRestrictingWrite;
    public int Count => variables.Count;
    private bool IsResultSet => variables.Count == 1 && variables.Single().IsResult;
    public bool IsWriteRestricted => variablesRestrictingWrite.Any();

    public SharingSet(bool isLent, FixedSet<ISharingVariable> variables, FixedSet<ISharingVariable> variablesRestrictingWrite)
    {
        IsLent = isLent;
        this.variables = variables.ToHashSet();
        this.variablesRestrictingWrite = variablesRestrictingWrite.ToHashSet();
    }

    public SharingSet(ISharingVariable variable, bool isLent)
    {
        IsLent = isLent;
        variables = new HashSet<ISharingVariable> { variable };
        variablesRestrictingWrite = new HashSet<ISharingVariable>();
    }

    public IEnumerator<ISharingVariable> GetEnumerator() => variables.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public SharingSetSnapshot Snapshot()
        => new(IsLent, variables.ToFixedSet(), variablesRestrictingWrite.ToFixedSet());

    public void Declare(ExternalReference lentGroup)
    {
        if (!IsLent) throw new InvalidOperationException("Cannot declare lent group for non-lent sharing set.");

        variables.Add(lentGroup);
    }

    public void Remove(ISharingVariable variable)
    {
        variables.Remove(variable);
        variablesRestrictingWrite.Remove(variable);
    }

    public void UnionWith(SharingSet smallerSet)
    {
        if ((IsLent && !smallerSet.IsResultSet)
            || (smallerSet.IsLent && !IsResultSet))
            throw new InvalidOperationException("Cannot union two sharing sets if either is lent unless they are result sets.");
        variables.UnionWith(smallerSet.variables);
        variablesRestrictingWrite.UnionWith(smallerSet.variablesRestrictingWrite);

    }

    public void RestrictsWriteTo(SharingSet restrictedSet)
        => throw new NotImplementedException();

    public void RestrictWrite(ISharingVariable variable) => variablesRestrictingWrite.Add(variable);

    public void Clear()
    {
        variables.Clear();
        variablesRestrictingWrite.Clear();
    }

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
        if (IsWriteRestricted) result += ", restrict write";
        return result;
    }
}
