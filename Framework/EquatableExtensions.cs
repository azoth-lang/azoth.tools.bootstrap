using System;

namespace Azoth.Tools.Bootstrap.Framework;

public static class EquatableExtensions
{
    public static EquatableEqualityComparer<T> EqualityComparer<T>(this T _) where T : IEquatable<T>?
        => EquatableEqualityComparer<T>.Instance;
}
