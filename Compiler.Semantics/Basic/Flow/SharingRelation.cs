using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// Stores the sharing relationship between the variables and the expression result. This
/// relationship is flow sensitive. Note that the sharing relationship is a partition of
/// the set of variables into disjoint subsets.
/// </summary>
public sealed class SharingRelation
{
    /// <summary>
    /// All the distinct subsets of variables
    /// </summary>
    private readonly HashSet<SharingSet> sets;
    /// <summary>
    /// This is a lookup of what set each variable is contained in.
    /// </summary>
    private readonly Dictionary<ISharingVariable, SharingSet> subsetFor;

    // TODO current result along different branches of if conflict?
    private ResultVariable? currentResult;

    // TODO current lend along different branches of if conflict?
    private ImplicitLend? currentLend;

    public SharingRelation()
    {
        sets = new();
        subsetFor = new();
    }

    internal SharingRelation(
        IEnumerable<SharingSetSnapshot> sets,
        ResultVariable? currentResult,
        ImplicitLend? currentLend)
    {
        this.currentResult = currentResult;
        this.currentLend = currentLend;
        this.sets = new(sets.Select(s => s.MutableCopy()));
        subsetFor = new();
        foreach (var set in this.sets)
            foreach (var variable in set)
                subsetFor.Add(variable, set);
    }

    private SharingRelation(IEnumerable<SharingSet> sets, ResultVariable? currentResult)
    {
        this.currentResult = currentResult;
        this.sets = new(sets);
        subsetFor = new();
        foreach (var set in this.sets)
            foreach (var variable in set)
                subsetFor.Add(variable, set);
    }

    public ResultVariable CurrentResult => currentResult
        ?? throw new InvalidOperationException("Cannot access current result because there is no current result.");

    public IEnumerable<IReadOnlySharingSet> SharingSets => sets;

    public IReadOnlySharingSet ReadSharingSet(ResultVariable variable) => SharingSet(variable);

    private SharingSet SharingSet(ISharingVariable variable)
    {
        if (!subsetFor.TryGetValue(variable, out var set))
            throw new InvalidOperationException($"Sharing variable {variable} no longer declared.");

        return set;
    }

    public ResultVariable LendConst(ResultVariable result)
    {
        _ = SharingSet(result);
        var borrowingResult = NewResult(lent: true);
        var lend = NewLend(restrictWrite: true);
        Declare(lend.From, false);
        Union(result, lend.From);
        Declare(lend.To, true);
        Union(borrowingResult, lend.To);
        return borrowingResult;
    }

    public SharingRelation Copy() => new(sets, currentResult);

    /// <summary>
    /// Declare a new variable. Newly created variables are not connected to any others.
    /// </summary>
    public void Declare(BindingVariable variable)
    {
        // Other types don't participate in sharing
        if (!variable.IsTracked) return;

        if (subsetFor.TryGetValue(variable, out _))
            throw new InvalidOperationException("Symbol already declared.");

        Declare(variable, variable.IsLent);
    }

    public void DeclareLentParameterReference(BindingVariable variable, uint lentParameterNumber)
    {
        if (!subsetFor.TryGetValue(variable, out var set))
            throw new InvalidOperationException("Cannot declare lent parameter for symbol not in a sharing set.");

        var lentParameter = ExternalReference.CreateLentParameter(lentParameterNumber);
        if (subsetFor.TryGetValue(lentParameter, out _))
            throw new InvalidOperationException($"Lent parameter {lentParameter} already declared");

        set.Declare(lentParameter);
        subsetFor.Add(lentParameter, set);
    }

    public void DeclareNonLentParametersReference()
    {
        if (subsetFor.TryGetValue(ExternalReference.NonParameters, out _))
            throw new InvalidOperationException("Non-lent parameters reference already declared.");

        Declare(ExternalReference.NonParameters, false);
    }

    private void Declare(ISharingVariable variable, bool isLent)
    {
        var set = new SharingSet(variable, isLent);
        sets.Add(set);
        subsetFor.Add(variable, set);
    }

    public ResultVariable NewResult(bool lent = false)
    {
        var newResult = currentResult?.NextResult() ?? ResultVariable.First;
        Declare(newResult, lent);
        currentResult = newResult;
        return newResult;
    }

    public IEnumerable<IReadOnlySharingSet> Drop(BindingVariable variable) => Drop((ISharingVariable)variable);

    public IEnumerable<IReadOnlySharingSet> Drop(ResultVariable result) => Drop((ISharingVariable)result);

    private IEnumerable<IReadOnlySharingSet> Drop(ISharingVariable variable)
    {
        if (!subsetFor.TryGetValue(variable, out var set))
            return Enumerable.Empty<IReadOnlySharingSet>();

        var affectedSets = new HashSet<IReadOnlySharingSet>();
        set.Remove(variable);
        subsetFor.Remove(variable);
        affectedSets.Add(set);
        if (!set.IsAlive) affectedSets.AddRange(DropAll(set));
        if (set.Count == 0) sets.Remove(set);
        if (variable is ImplicitLendTo lendTo)
            affectedSets.AddRange(Drop(lendTo.From));
        return affectedSets;
    }

    public FixedSet<IReadOnlySharingSet> DropAllLocalVariablesAndParameters()
        => DropAll(subsetFor.Keys.Where(v => v.IsVariableOrParameter)).ToFixedSet();

    private IEnumerable<IReadOnlySharingSet> DropAll(IEnumerable<ISharingVariable> sharingVariables)
        => sharingVariables.ToArray().SelectMany(Drop).Distinct();

    public void Union(ISharingVariable var1, BindingVariable var2)
        => Union(var1, (ISharingVariable)var2);

    public void Union(ISharingVariable var1, ISharingVariable var2)
    {
        if (!var1.IsTracked || !var2.IsTracked) return;
        var set1 = SharingSet(var1);
        var set2 = SharingSet(var2);
        if (set1 == set2)
            return;

        var (smallerSet, largerSet) = set1.Count <= set2.Count ? (set1, set2) : (set2, set1);
        largerSet.UnionWith(smallerSet);
        sets.Remove(smallerSet);
        foreach (var symbol in smallerSet)
            subsetFor[symbol] = largerSet;

        // To avoid bugs, clear out the smaller set that shouldn't be used anymore
        smallerSet.Clear();
    }

    /// <summary>
    /// Split the given variable out from sharing with the other variables it is connected to.
    /// </summary>
    public void Split(ResultVariable variable)
    {
        if (!subsetFor.TryGetValue(variable, out var set)
            || set.Count == 1)
            return;

        set.Remove(variable);
        var newSet = new SharingSet(variable, false);
        sets.Add(newSet);
        subsetFor[variable] = newSet;
    }

    public bool IsIsolated(ISharingVariable variable)
        => subsetFor.TryGetValue(variable, out var set) && set.IsIsolated;

    public bool IsIsolatedExcept(ISharingVariable variable, ResultVariable result)
        => subsetFor.TryGetValue(variable, out var set)
           && set.IsIsolatedExcept(result);

    public SharingRelationSnapshot Snapshot() => new(sets, currentResult, currentLend);

    public override string ToString()
        => string.Join(", ", sets.Select(s => $"{{{string.Join(", ", s.Distinct())}}}"));

    private ImplicitLend NewLend(bool restrictWrite)
        => currentLend = new ImplicitLend(currentLend, restrictWrite);
}
