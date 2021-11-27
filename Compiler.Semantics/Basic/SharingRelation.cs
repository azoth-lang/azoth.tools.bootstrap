using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic
{
    /// <summary>
    /// Stores the sharing relationship between the variables and the expression result. This
    /// relationship is flow sensitive. Note that the sharing relationship is a partition of
    /// the set of variables into disjoint subsets.
    /// </summary>
    public class SharingRelation
    {
        private readonly Dictionary<SharingVariable, ISet<SharingVariable>> sets;

        public SharingRelation()
        {
            // Construct a single element set for the result so that it exists when unioning
            sets = new() { { SharingVariable.Result, new HashSet<SharingVariable>() { SharingVariable.Result } } };
        }

        internal SharingRelation(IReadOnlyDictionary<SharingVariable, FixedSet<SharingVariable>> sets)
        {
            this.sets = sets.ToDictionary(pair => pair.Key, pair => (ISet<SharingVariable>)pair.Value.ToHashSet());
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
                sets.Add(symbol, new HashSet<SharingVariable>() { symbol });
        }

        public void Union(SharingVariable var1, SharingVariable var2)
        {
            if (!sets.TryGetValue(var1, out var set1)
                || !sets.TryGetValue(var2, out var set2)
                || set1 == set2)
                return;

            var (smallerSet, largerSet) = set1.Count <= set2.Count ? (set1, set2) : (set2, set1);
            largerSet.UnionWith(smallerSet);
            foreach (var symbol in smallerSet)
                sets[symbol] = largerSet;

            // To avoid bugs, clear out the smaller set that shouldn't be used anymore
            smallerSet.Clear();
        }

        /// <summary>
        /// Split the given variable out from sharing with the other variables it is connected to.
        /// </summary>
        public void Split(SharingVariable variable)
        {
            if (!sets.TryGetValue(variable, out var set)
                || set.Count == 1)
                return;

            set.Remove(variable);
            sets[variable] = new HashSet<SharingVariable> { variable };
        }

        public bool IsIsolated(SharingVariable variable)
            => sets.TryGetValue(variable, out var set) && set.Count == 1;

        public SharingRelationSnapshot Snapshot() => new(sets);
    }
}
