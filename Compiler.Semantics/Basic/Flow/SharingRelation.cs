using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
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

    private ResultVariable? currentResult;

    public SharingRelation()
    {
        sets = new();
        subsetFor = new();
    }

    internal SharingRelation(IEnumerable<SharingSetSnapshot> sets, ResultVariable? currentResult)
    {
        this.currentResult = currentResult;
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

    /// <summary>
    /// Restrict write to the sharing set of the variable via the variable.
    /// </summary>
    /// <returns>The set of affected symbols.</returns>
    public IEnumerable<BindingSymbol> RestrictWrite(ISharingVariable variable)
    {
        if (subsetFor.TryGetValue(variable, out var set))
        {
            set.RestrictWrite(variable);
            return set.OfType<BindingVariable>().Select(v => v.Symbol);
        }
        return Enumerable.Empty<BindingSymbol>();
    }

    public IEnumerable<BindingSymbol> RestrictWrite(ResultVariable resultToRestrict, ResultVariable restrictBy)
    {
        if (subsetFor.TryGetValue(resultToRestrict, out var restrictedSet)
            && subsetFor.TryGetValue(restrictBy, out var restrictBySet))
        {
            restrictBySet.RestrictsWriteTo(restrictedSet);
            restrictedSet.RestrictWrite(resultToRestrict);
            return restrictedSet.OfType<BindingVariable>().Select(v => v.Symbol);
        }

        return Enumerable.Empty<BindingSymbol>();
    }

    public SharingRelation Copy() => new(sets, currentResult);

    /// <summary>
    /// Declare a new variable. Newly created variables are not connected to any others.
    /// </summary>
    public void Declare(BindingVariable variable)
    {
        // Other types don't participate in sharing
        if (variable.DataType is not ReferenceType referenceType) return;

        if (subsetFor.TryGetValue(variable, out _))
            throw new InvalidOperationException("Symbol already declared.");

        var capability = referenceType.Capability;
        // No need to track `const` and `id`, they never participate in sharing because they don't
        // allow any write aliases. (Can't use AllowsWriteAliases here because of `iso`.)
        if (variable.IsLent
            || (capability != ReferenceCapability.Constant && capability != ReferenceCapability.Identity))
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

    public IReadOnlySharingSet? Drop(BindingVariable variable) => Drop((ISharingVariable)variable);


    public IReadOnlySharingSet? Drop(ResultVariable result) => Drop((ISharingVariable)result);

    private IReadOnlySharingSet? Drop(ISharingVariable variable)
    {
        if (!subsetFor.TryGetValue(variable, out var set))
            return null;

        set.Remove(variable);
        subsetFor.Remove(variable);
        if (set.Count == 0) sets.Remove(set);
        return set;
    }

    public void DropAllLocalVariablesAndParameters()
    {
        foreach (var variable in subsetFor.Keys.Where(v => v.IsVariableOrParameter).ToArray())
            Drop(variable);
    }

    public void UnionWithCurrentResult(ISharingVariable var) => Union(var, CurrentResult);

    public void UnionWithCurrentResultAndDrop(ResultVariable variable)
    {
        UnionWithCurrentResult(variable);
        Drop(variable);
    }

    public void Union(ISharingVariable var1, BindingVariable var2)
        => Union(var1, (ISharingVariable)var2);

    public void Union(ISharingVariable var1, ISharingVariable var2)
    {
        if (!subsetFor.TryGetValue(var1, out var set1)
            || !subsetFor.TryGetValue(var2, out var set2)
            || set1 == set2)
            return;

        var (smallerSet, largerSet) = set1.Count <= set2.Count ? (set1, set2) : (set2, set1);
        largerSet.UnionWith(smallerSet);
        sets.Remove(smallerSet);
        foreach (var symbol in smallerSet)
            subsetFor[symbol] = largerSet;

        // To avoid bugs, clear out the smaller set that shouldn't be used anymore
        smallerSet.Clear();
    }

    public void SplitCurrentResult() => Split(CurrentResult);

    /// <summary>
    /// Split the given variable out from sharing with the other variables it is connected to.
    /// </summary>
    private void Split(ISharingVariable variable)
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
        => subsetFor.TryGetValue(variable, out var set) && set.Count == 1;

    public bool IsIsolatedExceptCurrentResult(ISharingVariable variable)
        => subsetFor.TryGetValue(variable, out var set)
           && set.Count <= 2
           && set.Except(CurrentResult).Count() == 1;

    public SharingRelationSnapshot Snapshot() => new(sets, currentResult);

    public override string ToString()
        => string.Join(", ", sets.Select(s => $"{{{string.Join(", ", s.Distinct())}}}"));
}
