using System;
using System.Linq;
using System.Text.RegularExpressions;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// A name that could be the name of a type.
/// </summary>
[Closed(typeof(StandardName), typeof(SpecialTypeName))]
public abstract partial class TypeName : IEquatable<TypeName>
{
    public string Text { get; }

    /// <summary>
    /// Gets the number of generic type parameters of the name. Will be 0 for non-generic names.
    /// </summary>
    public int GenericParameterCount { get; }

    private protected TypeName(string text, int genericParameterCount)
    {
        Requires.That(nameof(text), !string.IsNullOrEmpty(text), "cannot be null or empty");
        Text = text;
        if (TokenTypes.Keywords.Contains(Text)
            || TokenTypes.ReservedWords.Contains(Text))
            QuotedText = '\\' + Text;
        else
        {
            text = text.Escape();
            QuotedText = NeedsQuoted().IsMatch(text) ? $@"\""{text}""" : text;
        }
        GenericParameterCount = genericParameterCount;
    }

    #region Equals
    public abstract bool Equals(TypeName? other);

    public sealed override bool Equals(object? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is TypeName otherTypeName && Equals(otherTypeName);
    }

    public abstract override int GetHashCode();

    public static bool operator ==(TypeName? left, TypeName? right) => Equals(left, right);

    public static bool operator !=(TypeName? left, TypeName? right) => !Equals(left, right);
    #endregion

    public abstract StandardName? WithAttributeSuffix();

    public abstract string ToBareString();

    public abstract override string ToString();

    public static implicit operator TypeName(string text) => new IdentifierName(text);

    protected string QuotedText { get; }

    [GeneratedRegex(@"[\\ #ₛ]")]
    private static partial Regex NeedsQuoted();
}
