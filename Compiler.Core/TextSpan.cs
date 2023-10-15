using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

[DebuggerDisplay("positions {Start} to {End}")]
public readonly struct TextSpan : IEquatable<TextSpan>
{
    public int Start { get; }
    public int End => Start + Length;
    public int Length { get; }
    public bool IsEmpty => Length == 0;

    public TextSpan(int start, int length)
    {
        Requires.Positive(nameof(start), start);
        Requires.Positive(nameof(length), length);
        Start = start;
        Length = length;
    }

    [System.Diagnostics.Contracts.Pure]
    public static TextSpan FromStartEnd(int start, int end)
    {
        Requires.Positive(nameof(start), start);
        Requires.Positive(nameof(end), end);
        return new TextSpan(start, end - start);
    }

    [System.Diagnostics.Contracts.Pure]
    public static TextSpan Covering(TextSpan left, TextSpan right)
        => FromStartEnd(Math.Min(left.Start, right.Start), Math.Max(left.End, right.End));

    [System.Diagnostics.Contracts.Pure]
    public static TextSpan Covering(TextSpan left, TextSpan? right)
    {
        if (right is not TextSpan y) return left;

        return Covering(left, y);
    }

    [System.Diagnostics.Contracts.Pure]
    public static TextSpan Covering(TextSpan? left, TextSpan right)
    {
        if (left is not TextSpan x) return right;

        return Covering(x, right);
    }

    [System.Diagnostics.Contracts.Pure]
    public static TextSpan? Covering(TextSpan? left, TextSpan? right)
    {
        if (left is not TextSpan xNew)
            return null;

        if (right is not TextSpan yNew)
            return xNew;

        return Covering(xNew, yNew);
    }

    [System.Diagnostics.Contracts.Pure]
    public static TextSpan Covering(params TextSpan?[] spans)
        => spans.Where(s => s is not null).Cast<TextSpan>().Aggregate(Covering);

    [System.Diagnostics.Contracts.Pure]
    public static TextSpan? Covering(IEnumerable<TextSpan> spans)
        => spans.Aggregate(default(TextSpan?), (left, right) => Covering(left, right));

    /// <summary>
    /// Returns a zero length span that occurs at the start of the current span
    /// </summary>
    public TextSpan AtStart() => new(Start, 0);

    /// <summary>
    /// Returns a zero length span that occurs at the end of the current span
    /// </summary>
    [System.Diagnostics.Contracts.Pure]
    public TextSpan AtEnd() => new(End, 0);

    [System.Diagnostics.Contracts.Pure]

    public string GetText(string text) => text.Substring(Start, Length);

    #region Equality
    [System.Diagnostics.Contracts.Pure]
    public override bool Equals(object? obj) => obj is TextSpan span && Equals(span);

    [System.Diagnostics.Contracts.Pure]
    public bool Equals(TextSpan other) => Start == other.Start && Length == other.Length;

    [System.Diagnostics.Contracts.Pure]
    public override int GetHashCode() => HashCode.Combine(Start, Length);

    [System.Diagnostics.Contracts.Pure]
    public static bool operator ==(TextSpan span1, TextSpan span2) => span1.Equals(span2);

    [System.Diagnostics.Contracts.Pure]
    public static bool operator !=(TextSpan span1, TextSpan span2) => !span1.Equals(span2);

    #endregion

    public override string ToString() => $"TextSpan({Start},{Length})";
}
