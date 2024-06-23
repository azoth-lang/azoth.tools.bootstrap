using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Framework;

public static class CollectionExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<TKey, TElement>(this ILookup<TKey, TElement> source)
        => source.Count == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(this IReadOnlyCollection<T> source)
        => source.Count == 0;
}
