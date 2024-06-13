using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework;

public static class CollectionExtensions
{
    public static bool IsEmpty<TKey, TElement>(this ILookup<TKey, TElement> source)
        => source.Count == 0;

    public static bool IsEmpty<T>(this IReadOnlyCollection<T> source)
        => source.Count == 0;
}
