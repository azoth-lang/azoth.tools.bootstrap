using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic
{
    /// <summary>
    /// An immutable snapshot of the sharing relationship between variables and the
    /// expression result at one point in the code.
    /// </summary>
    public class SharingRelationSnapshot
    {
        private readonly FixedDictionary<SharingVariable, FixedSet<SharingVariable>> sets;

        internal SharingRelationSnapshot(IDictionary<SharingVariable, ISet<SharingVariable>> sets)
        {
            this.sets = sets.ToFixedDictionary(pair => pair.Key, pair => pair.Value.ToFixedSet());
        }

        public SharingRelation MutableCopy() => new(sets);
    }
}
