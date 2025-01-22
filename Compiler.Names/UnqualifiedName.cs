using System;
using System.Linq;
using System.Text.RegularExpressions;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// A name that is not qualified. That is it has no context preceding it with a dot.
/// </summary>
[Closed(typeof(OrdinaryName), typeof(BuiltInTypeName))]
public abstract partial class UnqualifiedName : IEquatable<UnqualifiedName>
{
    public string Text { get; }

    /// <summary>
    /// Gets the number of generic type parameters of the name. Will be 0 for non-generic names.
    /// </summary>
    public int GenericParameterCount { get; }

    private protected UnqualifiedName(string text, int genericParameterCount)
    {
        Requires.That(!string.IsNullOrEmpty(text), nameof(text), "cannot be null or empty");
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
    public abstract bool Equals(UnqualifiedName? other);

    public sealed override bool Equals(object? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is UnqualifiedName otherTypeName && Equals(otherTypeName);
    }

    public abstract override int GetHashCode();

    public static bool operator ==(UnqualifiedName? left, UnqualifiedName? right) => Equals(left, right);

    public static bool operator !=(UnqualifiedName? left, UnqualifiedName? right) => !Equals(left, right);
    #endregion

    public abstract OrdinaryName? WithAttributeSuffix();

    /// <summary>
    /// The name without any generic parameters.
    /// </summary>
    public abstract string ToBareString();

    public abstract override string ToString();

    public static implicit operator UnqualifiedName(string text) => new IdentifierName(text);

    protected string QuotedText { get; }

    [GeneratedRegex(@"[\\ #ₛ]")]
    private static partial Regex NeedsQuoted();
}
