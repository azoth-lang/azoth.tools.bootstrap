using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;

/// <summary>
/// Stores the sharing relationship between the variables and the expression result. This
/// relationship is flow sensitive. Note that the sharing relationship is a partition of
/// the set of variables into disjoint subsets.
/// </summary>
public class SharingRelation
{
    /// <summary>
    /// All the distinct subsets of variables
    /// </summary>
    private readonly HashSet<ISet<SharingVariable>> sets;
    /// <summary>
    /// This is a lookup of what set each variable is contained in.
    /// </summary>
    private readonly Dictionary<SharingVariable, ISet<SharingVariable>> subsetFor;

    public SharingRelation()
    {
        sets = new();
        // Construct a single element set for the result so that it exists when unioning
        var set = new HashSet<SharingVariable> { SharingVariable.Result };
        sets.Add(set);
        subsetFor = new() { { SharingVariable.Result, set } };
    }

    internal SharingRelation(IEnumerable<FixedSet<SharingVariable>> sets)
    {
        this.sets = new(sets.Select(s => s.ToHashSet()));
        subsetFor = new();
        foreach (var set in this.sets)
            foreach (var variable in set)
                subsetFor.Add(variable, set);
    }

    /// <summary>
    /// Declare a new variable. Newly created variables are not connected to any others.
    /// </summary>
    public void Declare(BindingSymbol symbol)
    {
        // Other types don't participate in sharing
        if (symbol.DataType is not ReferenceType referenceType) return;

        var capability = referenceType.Capability;
        // No need to track `const` and `id`, they never participate in sharing
        if (capability != ReferenceCapability.Constant
            && capability != ReferenceCapability.Identity)
        {
            var set = new HashSet<SharingVariable> { symbol };
            sets.Add(set);
            subsetFor.Add(symbol, set);
        }
    }

    public void Drop(BindingSymbol symbol)
    {
        var variable = (SharingVariable)symbol;
        if (subsetFor.TryGetValue(variable, out var set))
        {
            set.Remove(variable);
            subsetFor.Remove(variable);
            if (set.Count == 0) sets.Remove(set);
        }
    }

    public void Union(SharingVariable var1, SharingVariable var2)
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

    public void UnionResult(SharingVariable? var)
    {
        if (var is SharingVariable sharingVar)
            Union(SharingVariable.Result, sharingVar);
    }

    /// <summary>
    /// Split the given variable out from sharing with the other variables it is connected to.
    /// </summary>
    public void Split(SharingVariable variable)
    {
        if (!subsetFor.TryGetValue(variable, out var set)
            || set.Count == 1)
            return;

        set.Remove(variable);
        var newSet = new HashSet<SharingVariable> { variable };
        sets.Add(newSet);
        subsetFor[variable] = newSet;
    }

    /// <summary>
    /// Split out any local variable or `iso` parameter since those references are out of scope.
    /// </summary>
    public void SplitForReturn()
    {
        foreach (var var in subsetFor.Keys)
            if (var.IsLocal || var.DataType is ReferenceType { IsIsolatedReference: true })
                Split(var);
    }

    public bool IsIsolated(SharingVariable variable)
        => subsetFor.TryGetValue(variable, out var set) && set.Count == 1;

    public SharingRelationSnapshot Snapshot() => new(sets);

    public override string ToString()
        => string.Join(", ", sets.Select(s => $"{{{string.Join(", ", s.Distinct())}}}"));
}
