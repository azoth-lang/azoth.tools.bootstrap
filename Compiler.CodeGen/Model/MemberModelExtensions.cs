using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public static class MemberModelExtensions
{
    public static bool IsObjectMember(this IMemberModel model)
        => ObjectMembers.Contains(model.Name);

    private static readonly IFixedSet<string> ObjectMembers = new HashSet<string>()
    {
        "ToString", "GetHashCode", "Equals",
    }.ToFixedSet();


    /// <summary>
    /// Get the distinct members by name, but prefer placeholders to actual members. Placeholders
    /// will appear in the result in the location of the first occurence of the placeholder even if
    /// there are other members with the same name before it.
    /// </summary>
    public static IEnumerable<T> DistinctKeepingPlaceholders<T>(this IEnumerable<T> source)
        where T : IMemberModel
        => PreferPlaceholders(source).DistinctBy(m => m.Name);

    public static IEnumerable<T> PreferPlaceholders<T>(this IEnumerable<T> source)
        where T : IMemberModel
    {
        var members = source.ToList();
        var placeholderNames = members.Where(m => m.IsPlaceholder).Select(m => m.Name).ToHashSet();
        return members.Where(m => m.IsPlaceholder || !placeholderNames.Contains(m.Name));
    }

    public static IEnumerable<T> ExceptPlaceholders<T>(this IEnumerable<T> source)
        where T : IMemberModel
        => source.Where(m => !m.IsPlaceholder);
}
