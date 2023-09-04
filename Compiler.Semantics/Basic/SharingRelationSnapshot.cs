using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;

/// <summary>
/// An immutable snapshot of the sharing relationship between variables and the
/// expression result at one point in the code.
/// </summary>
public class SharingRelationSnapshot
{
    private readonly FixedSet<FixedSet<SharingVariable>> sets;

    internal SharingRelationSnapshot(ISet<ISet<SharingVariable>> sets)
    {
        this.sets = new(sets.Select(s => s.ToFixedSet()));
    }

    public SharingRelation MutableCopy() => new(sets);
}
