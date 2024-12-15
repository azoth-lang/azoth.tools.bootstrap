using System;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// A simple name. That is, a name that is not qualified and does not have generic parameters.
/// </summary>
public sealed class IdentifierName : OrdinaryName
{
    public IdentifierName(string text)
        : base(text, 0) { }

    #region Equals
    public override bool Equals(TypeName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is IdentifierName otherName
               && Text == otherName.Text;
    }

    public override int GetHashCode()
        => HashCode.Combine(typeof(IdentifierName), Text);
    #endregion

    public override OrdinaryName WithAttributeSuffix() => Text + SpecialNames.AttributeSuffix;

    public override string ToString() => QuotedText;

    public static implicit operator IdentifierName(string text)
        => new(text);
}
