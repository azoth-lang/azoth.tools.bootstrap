using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Azoth.Tools.Bootstrap.Framework;

/// <summary>
/// Note: The parameters for the parameter names are intentionally named
/// `parameter` rather than `name` so that VS autocomplete won't try to
/// complete to `name:` when you type `nameof...`
/// </summary>
public static class Requires
{
    [DebuggerHidden]
    public static void Positive(int value, string paramName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Must be greater than or equal to zero");
    }

    [DebuggerHidden]
    public static void InString(string paramName, string inString, int value)
    {
        // Start is allowed to be equal to length to allow for a zero length span after the last character
        if (value < 0 || value >= inString.Length)
            throw new ArgumentOutOfRangeException(paramName, value, $"Value not in string of length {inString.Length}");
    }

    [DebuggerHidden]
    public static void ValidEnum<TEnum>(TEnum value, string paramName)
        where TEnum : struct, Enum
    {
        if (!value.IsDefined())
            throw new InvalidEnumArgumentException(paramName, Convert.ToInt32(value, CultureInfo.InvariantCulture), typeof(TEnum));
    }

    [DebuggerHidden]
    public static void NotNullOrEmpty(string value, string paramName)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be null or empty", paramName);
    }

    [DebuggerHidden]
    public static void That([DoesNotReturnIf(false)] bool condition, string paramName, string message)
    {
        if (!condition)
            throw new ArgumentException(message, paramName);
    }

    public static void Zero<T>(T value, string paramName)
        where T : IAdditiveIdentity<T, T>, IEquatable<T>
    {
        if (!T.AdditiveIdentity.Equals(value))
            throw new ArgumentException(MustBeZero, paramName);
    }

    private const string MustBeZero = "Must be zero.";
}
