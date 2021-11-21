using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic
{
    /// <summary>
    /// An immutable snapshot of the sharing relationship between variables and the
    /// expression result at one point in the code.
    /// </summary>
    public class SharingRelationSnapshot
    {
        private readonly FixedDictionary<BindingSymbol, FixedSet<BindingSymbol>> sets;

        public SharingRelationSnapshot(IDictionary<BindingSymbol, ISet<BindingSymbol>> sets)
        {
            this.sets = sets.ToFixedDictionary(pair => pair.Key, pair => pair.Value.ToFixedSet());
        }

        public SharingRelation MutableCopy() => new SharingRelation(sets);
    }
}
