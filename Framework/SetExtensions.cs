using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework;

public static class SetExtensions
{
    public static void AddRange<T>(this ISet<T> set, IEnumerable<T> values)
    {
        foreach (var value in values) set.Add(value);
    }

    public static bool TryTake<T>(this ISet<T> set, out T value)
    {
        if (set.Count == 0)
        {
            value = default!;
            return false;
        }

        value = set.First();
        set.Remove(value);
        return true;
    }
}
