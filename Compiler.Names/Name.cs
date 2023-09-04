using System;
using System.Linq;
using System.Text.RegularExpressions;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// A standard name
/// </summary>
public sealed partial class Name : TypeName
{
    public Name(string text)
        : base(text) { }

    public override bool Equals(TypeName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is Name otherName
               && Text == otherName.Text;
    }

    public override int GetHashCode()
        => HashCode.Combine(typeof(Name), Text);

    public override string ToString()
    {
        if (TokenTypes.Keywords.Contains(Text)
            || TokenTypes.ReservedWords.Contains(Text))
            return '\\' + Text;

        var text = Text.Escape();
        if (NeedsQuoted().IsMatch(text)) text += $@"\""{text}""";
        return text;
    }

    public static implicit operator Name(string text)
        => new(text);

    [GeneratedRegex(@"[\\ #ₛ]")]
    private static partial Regex NeedsQuoted();
}
