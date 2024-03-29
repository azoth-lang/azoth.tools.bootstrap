using System;
using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Config;

[DebuggerDisplay("{" + nameof(ToString) + ",nq}")]
public sealed class GrammarSymbol : IEquatable<GrammarSymbol>
{
    public string Text { get; }
    public bool IsQuoted { get; }

    public GrammarSymbol(string text, bool isQuoted = false)
    {
        Text = text;
        IsQuoted = isQuoted;
    }

    public bool Equals(GrammarSymbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Text == other.Text
               && IsQuoted == other.IsQuoted;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is GrammarSymbol other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(Text, IsQuoted);

    public static bool operator ==(GrammarSymbol? left, GrammarSymbol? right) => Equals(left, right);

    public static bool operator !=(GrammarSymbol? left, GrammarSymbol? right) => !Equals(left, right);

    public override string ToString() => IsQuoted ? $"'{Text}'" : Text;
}
