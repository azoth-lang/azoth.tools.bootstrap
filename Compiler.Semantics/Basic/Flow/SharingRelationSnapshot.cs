using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// An immutable snapshot of the sharing relationship between variables and the
/// expression result at one point in the code.
/// </summary>
public class SharingRelationSnapshot
{
    private readonly FixedSet<SharingSetSnapshot> sets;

    internal SharingRelationSnapshot(IEnumerable<SharingSet> sets)
    {
        this.sets = new(sets.Select(s => s.Snapshot()));
    }

    public SharingRelation MutableCopy() => new(sets);
}
