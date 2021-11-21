using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic
{
    /// <summary>
    /// An immutable snapshot of the sharing relationship between variables and the
    /// expression result at one point in the code.
    /// </summary>
    public class FlowSharingSnapshot
    {
        private readonly FixedDictionary<BindingSymbol, FixedSet<BindingSymbol>> sets;

        public FlowSharingSnapshot(IDictionary<BindingSymbol, ISet<BindingSymbol>> sets)
        {
            this.sets = sets.ToFixedDictionary(pair => pair.Key, pair => pair.Value.ToFixedSet());
        }

        public FlowSharing MutableCopy() => new FlowSharing(sets);
    }
}
