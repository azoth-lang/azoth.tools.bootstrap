using System;

namespace Azoth.Tools.Bootstrap.Framework;

/// <summary>
/// An ordering of two items. Returned by methods like
/// <see cref="IOrderedOrdered{TSelf}.CompareTo"/>. Supports both properties for checking
/// what the comparison was and the idiom of <c>a.CompareTo(b) >= 0</c>.
/// </summary>
public readonly struct Ordering : IEquatable<Ordering>
{
    public static readonly Ordering Less = new(-1);
    public static readonly Ordering Equal = new(0);
    public static readonly Ordering Greater = new(1);

    private Ordering(int value)
    {
        this.value = Math.Sign(value);
    }

    public bool IsLess => value < 0;
    public bool IsLessThanOrEqual => value <= 0;
    public bool IsEqual => value == 0;
    public bool IsGreater => value > 0;
    public bool IsGreaterThanOrEqual => value >= 0;

    #region Equality
    public bool Equals(Ordering other) => value == other.value;

    public override bool Equals(object? obj) => obj is Ordering other && Equals(other);

    public override int GetHashCode() => value;

    public static bool operator ==(Ordering left, Ordering right) => left.Equals(right);

    public static bool operator !=(Ordering left, Ordering right) => !left.Equals(right);
    #endregion

    #region Operators
    /// <summary>
    /// Allows <c>a.CompareTo(b) == 0</c> to check if <c>a == b</c>.
    /// </summary>
    public static bool operator ==(Ordering left, int zero)
    {
        Requires.Zero(zero, nameof(zero));
        return left.value == zero;
    }

    /// <summary>
    /// Allows <c>a.CompareTo(b) != 0</c> to check if <c>a != b</c>.
    /// </summary>
    public static bool operator !=(Ordering left, int zero)
    {
        Requires.Zero(zero, nameof(zero));
        return left.value != zero;
    }

    /// <summary>
    /// Allows <c>a.CompareTo(b) &lt; 0</c> to check if <c>a &lt; b</c>.
    /// </summary>
    public static bool operator <(Ordering left, int zero)
    {
        Requires.Zero(zero, nameof(zero));
        return left.value < zero;
    }

    /// <summary>
    /// Allows <c>a.CompareTo(b) &lt;= 0</c> to check if <c>a &lt;= b</c>.
    /// </summary>
    public static bool operator <=(Ordering left, int zero)
    {
        Requires.Zero(zero, nameof(zero));
        return left.value <= zero;
    }

    /// <summary>
    /// Allows <c>a.CompareTo(b) > 0</c> to check if <c>a > b</c>.
    /// </summary>
    public static bool operator >(Ordering left, int zero)
    {
        Requires.Zero(zero, nameof(zero));
        return left.value > zero;
    }

    /// <summary>
    /// Allows <c>a.CompareTo(b) >= 0</c> to check if <c>a >= b</c>.
    /// </summary>
    public static bool operator >=(Ordering left, int zero)
    {
        Requires.Zero(zero, nameof(zero));
        return left.value >= zero;
    }
    #endregion

    public static explicit operator Ordering(int ordering)
        => new(ordering);

    public static explicit operator int(Ordering ordering)
        => ordering.value;

    private readonly int value;
}
