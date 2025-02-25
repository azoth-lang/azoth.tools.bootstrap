using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using JetBrains.Annotations;
using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace Azoth.Tools.Bootstrap.Framework;

/// <summary>
/// Note: The parameters for the parameter names are intentionally named
/// `paramName` rather than `name` so that VS autocomplete won't try to
/// complete to `name:` when you type `nameof...`
/// </summary>
public static class Requires
{
    [DebuggerHidden]
    [Conditional("DEBUG")]
    public static void Positive(int value, [InvokerParameterName] string paramName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Must be greater than or equal to zero");
    }

    [DebuggerHidden]
    [Conditional("DEBUG")]
    public static void InString([InvokerParameterName] string paramName, string inString, int value)
    {
        // Start is allowed to be equal to length to allow for a zero length span after the last character
        if (value < 0 || value >= inString.Length)
            throw new ArgumentOutOfRangeException(paramName, value, $"Value not in string of length {inString.Length}");
    }

    [DebuggerHidden]
    [Conditional("DEBUG")]
    public static void ValidEnum<TEnum>(TEnum value, [InvokerParameterName] string paramName)
        where TEnum : struct, Enum
    {
        if (!value.IsDefined())
            throw new InvalidEnumArgumentException(paramName, Convert.ToInt32(value, CultureInfo.InvariantCulture), typeof(TEnum));
    }

    [DebuggerHidden]
    [Conditional("DEBUG")]
    public static void Null<T>(T? value, [InvokerParameterName] string paramName, string? message)
        where T : class
    {
        if (value is not null)
            throw new ArgumentException(message, paramName);
    }

    [DebuggerHidden]
    [Conditional("DEBUG")]
    public static void NotNull<T>([NotNull] T? value, [InvokerParameterName] string paramName)
        where T : class
    {
        if (value is null)
            throw new ArgumentException("Value cannot be null.", paramName);
    }

    [DebuggerHidden]
    [Conditional("DEBUG")]
    public static void NotNullOrEmpty([NotNull] string? value, [InvokerParameterName] string paramName)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be null or empty.", paramName);
    }

    [DebuggerHidden]
    [Conditional("DEBUG")]
    public static void That([DoesNotReturnIf(false)] bool condition, [InvokerParameterName] string paramName, string message)
    {
        if (!condition)
            throw new ArgumentException(message, paramName);
    }

    [DebuggerHidden]
    [Conditional("DEBUG")]
    public static void Zero<T>(T value, [InvokerParameterName] string paramName)
        where T : IAdditiveIdentity<T, T>, IEquatable<T>
    {
        if (!T.AdditiveIdentity.Equals(value))
            throw new ArgumentException(MustBeZero, paramName);
    }

    private const string MustBeZero = "Must be zero.";
}
