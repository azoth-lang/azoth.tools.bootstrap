using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal static class DeclarationNodeExtensions
{
    public static IEnumerable<T> MembersNamed<T>(
        this IEnumerable<T> members,
        ref MultiMapHashSet<StandardName, T>? membersByName,
        StandardName named)
        where T : IFacetChildDeclarationNode
    {
        membersByName ??= new(members.GroupBy(m => m.Name).Where(g => g.Key is not null)
                                     .ToDictionary(g => g.Key!, g => g.ToHashSet()));

        return membersByName.TryGetValue(named, out var values)
            ? values : Enumerable.Empty<T>();
    }
}
