using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// A name that has generic parameters.
/// </summary>
public sealed class GenericName : OrdinaryName
{
    public GenericName(string text, int genericParameterCount)
        : base(text, genericParameterCount)
    {
        Requires.That(genericParameterCount > 0, nameof(genericParameterCount), "Must be greater than zero.");
    }

    #region Equals
    public override bool Equals(UnqualifiedName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OrdinaryName otherName
               && Text == otherName.Text
               && GenericParameterCount == otherName.GenericParameterCount;
    }

    public override int GetHashCode() => HashCode.Combine(Text, GenericParameterCount);
    #endregion

    public override OrdinaryName WithAttributeSuffix()
        => new GenericName(SpecialNames.WithAttributeSuffix(Text), GenericParameterCount);

    public override string ToString() => $"{QuotedText}[{new string(',', GenericParameterCount - 1)}]";
}
