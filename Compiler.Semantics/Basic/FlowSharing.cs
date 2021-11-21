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
    public class FlowSharing
    {
        private readonly Dictionary<BindingSymbol, ISet<BindingSymbol>> sets;

        public FlowSharing()
        {
            sets = new();
        }

        public FlowSharing(IReadOnlyDictionary<BindingSymbol, FixedSet<BindingSymbol>> sets)
        {
            this.sets = sets.ToDictionary(pair => pair.Key, pair => (ISet<BindingSymbol>)pair.Value.ToHashSet());
        }

        /// <summary>
        /// Declare a new variable. Newly created variables are not connected to any others.
        /// </summary>
        public void Declare(BindingSymbol symbol)
        {
            // Other types don't participate in sharing
            if (symbol.DataType is not ReferenceType referenceType) return;

            var capability = referenceType.Capability;
            if (capability != ReferenceCapability.Constant
                && capability != ReferenceCapability.Identity)
                sets.Add(symbol, new HashSet<BindingSymbol>() { symbol });
        }

        public void Union(BindingSymbol symbol1, BindingSymbol symbol2)
        {
            if (!sets.TryGetValue(symbol1, out var set1)
                || !sets.TryGetValue(symbol2, out var set2)
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
        /// Split the given symbol out from sharing with the other variables it is connected to.
        /// </summary>
        public void Split(BindingSymbol symbol)
        {
            if (!sets.TryGetValue(symbol, out var set)
                || set.Count == 1)
                return;

            set.Remove(symbol);
            sets[symbol] = new HashSet<BindingSymbol> { symbol };
        }

        public bool IsIsolated(BindingSymbol symbol)
            => sets.TryGetValue(symbol, out var set) && set.Count == 1;

        public FlowSharingSnapshot Snapshot() => new FlowSharingSnapshot(sets);
    }
}
