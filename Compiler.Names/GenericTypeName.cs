using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

public sealed class GenericTypeName : StandardTypeName
{
    public GenericTypeName(string text, int genericParameterCount)
        : base(text, genericParameterCount)
    {
        Requires.That(nameof(genericParameterCount), genericParameterCount > 0, "Must be greater than zero.");
    }

    #region Equals
    public override bool Equals(TypeName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is StandardTypeName otherName
               && Text == otherName.Text
               && GenericParameterCount == otherName.GenericParameterCount;
    }

    public override int GetHashCode() => HashCode.Combine(Text, GenericParameterCount);
    #endregion

    public override StandardTypeName WithAttributeSuffix()
        => new GenericTypeName(Text + SpecialNames.AttributeSuffix, GenericParameterCount);

    public override string ToString() => $"{QuotedText}[{new string(',', GenericParameterCount - 1)}]";
}
