using System;

namespace Azoth.Tools.Bootstrap.Framework;

public static class Enums
{
    public static T Min<T>(this T left, T right)
        where T : struct, Enum
        => left.CompareTo(right) <= 0 ? left : right;
}
