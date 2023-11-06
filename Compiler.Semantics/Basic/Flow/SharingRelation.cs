using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
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

    public SharingRelation()
    {
        sets = new();
        subsetFor = new();
    }

    internal SharingRelation(IEnumerable<SharingSetSnapshot> sets)
    {
        this.sets = new(sets.Select(s => s.MutableCopy()));
        subsetFor = new();
        foreach (var set in this.sets)
            foreach (var variable in set)
                subsetFor.Add(variable, set);
    }

    private SharingRelation(IEnumerable<SharingSet> sets)
    {
        this.sets = new(sets.Select(s => new SharingSet(s)));
        subsetFor = new();
        foreach (var set in this.sets)
            foreach (var variable in set)
                subsetFor.Add(variable, set);
    }

    public IEnumerable<IReadOnlySharingSet> SharingSets => sets;

    public IReadOnlySharingSet ReadSharingSet(ResultVariable variable) => SharingSet(variable);

    private SharingSet SharingSet(BindingVariable variable) => SharingSet((ISharingVariable)variable);
    private SharingSet SharingSet(ISharingVariable variable)
    {
        if (!subsetFor.TryGetValue(variable, out var set))
            throw new InvalidOperationException($"Sharing variable {variable} no longer declared.");

        return set;
    }

    public ResultVariable LendConst(
        ResultVariable result,
        ResultVariableFactory resultVariableFactory,
        ImplicitLendFactory implicitLendFactory)
    {
        _ = SharingSet(result);
        var borrowingResult = NewResult(resultVariableFactory, isLent: true);
        var lend = implicitLendFactory.CreateConstLend();
        Declare(lend.From, false);
        Union(result, lend.From);
        Declare(lend.To, true);
        Union(borrowingResult, lend.To);
        return borrowingResult;
    }

    public ResultVariable LendIso(
        ResultVariable result,
        ResultVariableFactory resultVariableFactory,
        ImplicitLendFactory implicitLendFactory)
    {
        _ = SharingSet(result);
        var borrowingResult = NewResult(resultVariableFactory, isLent: true);
        var lend = implicitLendFactory.CreateIsoLend();
        Declare(lend.From, false);
        Union(result, lend.From);
        Declare(lend.To, true);
        Union(borrowingResult, lend.To);
        return borrowingResult;
    }

    public SharingRelation Copy() => new(sets);

    /// <summary>
    /// Declare a new variable. Newly created variables are not connected to any others.
    /// </summary>
    /// <returns>Whether the variable was declared.</returns>
    public BindingVariable? Declare(BindingSymbol symbol)
    {
        // Other types don't participate in sharing
        if (!symbol.SharingIsTracked()) return null;

        BindingVariable variable = symbol;
        if (subsetFor.TryGetValue(variable, out _))
            throw new InvalidOperationException("Symbol already declared.");

        Declare(variable, variable.IsLent);
        return variable;
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
        if (subsetFor.TryGetValue(ExternalReference.NonLentParameters, out _))
            throw new InvalidOperationException("Non-lent parameters reference already declared.");

        Declare(ExternalReference.NonLentParameters, false);
    }

    private void Declare(ISharingVariable variable, bool isLent)
    {
        var set = new SharingSet(variable, isLent);
        sets.Add(set);
        subsetFor.Add(variable, set);
    }

    // TODO remove
    private ResultVariable NewResult(ResultVariableFactory resultVariableFactory, bool isLent = false)
    {
        var result = resultVariableFactory.Create();
        Declare(result, isLent);
        return result;
    }

    /// <summary>
    /// Create a new result inside the set with the given symbol.
    /// </summary>
    public ResultVariable? NewResult(BindingSymbol symbol, ResultVariableFactory resultVariableFactory)
    {
        if (!symbol.SharingIsTracked()) return null;
        var set = SharingSet(symbol);

        var result = resultVariableFactory.Create();
        set.Declare(result);
        subsetFor.Add(result, set);
        return result;
    }

    public IEnumerable<IReadOnlySharingSet> Drop(BindingVariable variable)
        => Drop((ISharingVariable)variable);

    public IEnumerable<IReadOnlySharingSet> Drop(ResultVariable result)
        => Drop((ISharingVariable)result);

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
        if (!var1.SharingIsTracked || !var2.SharingIsTracked) return;
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

    public bool IsIsolated(ISharingVariable variable)
        => subsetFor.TryGetValue(variable, out var set) && set.IsIsolated;

    public bool IsIsolatedExceptFor(ISharingVariable variable, ResultVariable result)
        => subsetFor.TryGetValue(variable, out var set) && set.IsIsolatedExceptFor(result);

    public SharingRelationSnapshot Snapshot() => new(sets);

    public override string ToString()
        => string.Join(", ", sets.Select(s => $"{{{string.Join(", ", s.Distinct())}}}"));

    public void Merge(SharingRelation other)
    {
        foreach (var set in other.SharingSets)
        {
            var representative = set.FirstOrDefault(subsetFor.ContainsKey);
            if (representative is null)
            {
                // Whole set is missing, copy and add it
                var newSet = new SharingSet(set);
                sets.Add(newSet);
                foreach (var variable in newSet)
                    subsetFor.Add(variable, newSet);
                continue;
            }

            // At least one item of the set is present. Use that part as a core to union everything
            // in the set together. This is necessary because there could be multiple sets that need
            // unioned.
            var targetSet = SharingSet(representative);
            foreach (var variable in set)
            {
                // If a variable is completely missing, just declare it. This ensures existing logic
                // can apply to the union.
                if (!subsetFor.ContainsKey(variable))
                    Declare(variable, set.IsLent);

                Union(representative, variable);
            }
        }
    }
}
