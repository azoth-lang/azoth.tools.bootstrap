using System;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// A simple name that is not a type name.
/// </summary>
public sealed class SimpleName : StandardTypeName
{
    public SimpleName(string text)
        : base(text, 0) { }

    #region Equals
    public override bool Equals(Name? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SimpleName otherName
               && Text == otherName.Text;
    }

    public override int GetHashCode()
        => HashCode.Combine(typeof(SimpleName), Text);
    #endregion

    public override string ToString() => QuotedText;

    public static implicit operator SimpleName(string text)
        => new(text);
}
