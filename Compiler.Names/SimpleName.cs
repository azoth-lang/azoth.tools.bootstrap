using System;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// A simple name. That is, a name that is not qualified and does not have generic parameters.
/// </summary>
public sealed class SimpleName : StandardTypeName
{
    public SimpleName(string text)
        : base(text, 0) { }

    #region Equals
    public override bool Equals(TypeName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SimpleName otherName
               && Text == otherName.Text;
    }

    public override int GetHashCode()
        => HashCode.Combine(typeof(SimpleName), Text);
    #endregion

    public override StandardTypeName WithAttributeSuffix() => Text + SpecialNames.AttributeSuffix;

    public override string ToString() => QuotedText;

    public static implicit operator SimpleName(string text)
        => new(text);
}
